//此测试shader来自雨轩，假如需要分享请备注雨轩的知乎主页链接https://www.zhihu.com/people/yu-xuan-68-77
#ifndef MYPBR_CGINCLUDED
    #define MYPBR_CGINCLUDED
    
    half4 BRDF1_Unity_PBS1(half3 diffColor, half3 specColor, half oneMinusReflectivity, half smoothness, float3 normal, float3 viewDir, UnityLight light, UnityIndirect gi)
    {
        float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);//=1-smoothness，光滑度转粗糙度。
        float3 halfDir = Unity_SafeNormalize(float3(light.dir) + viewDir);//求半角向量。Unity_SafeNormalize函数用于避免出现除零及负数的情况。
        
        // NdotV should not be negative for visible pixels, but it can happen due to perspective projection and normal mapping
        // In this case normal should be modified to become valid (i.e facing camera) and not cause weird artifacts.
        // but this operation adds few ALU and users may not want it. Alternative is to simply take the abs of NdotV (less correct but works too).
        // Following define allow to control this. Set it to 0 if ALU is critical on your platform.
        // This correction is interesting for GGX with SmithJoint visibility function because artifacts are more visible in this case due to highlight edge of rough surface
        // Edit: Disable this code by default for now as it is not compatible with two sided lighting used in SpeedTree.
        // 以上部分简单翻译：
        // 1、要避免dot(normal,viewDir)为负。但透视视角和法线贴图映射时有可能出现这种为负情况。
        // 2、解决这个问题提供了两种方案。1>把法线扭到偏向摄影机方向再做点积计算（准确但耗性能）2>直接对点积取绝对值（不完全准确，但效果可接受，省性能）
        #define UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV 0//默认情况下走方法2，假如要走方法1需要注释此行
        
        #if UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV//方法1
            // The amount we shift the normal toward the view vector is defined by the dot product.
            half shiftAmount = dot(normal, viewDir);
            normal = shiftAmount < 0.0f ? normal + viewDir * (-shiftAmount + 1e-5f): normal;
            // A re-normalization should be applied here but as the shift is small we don't do it to save ALU.
            //normal = normalize(normal);
            float nv = saturate(dot(normal, viewDir)); // TODO: this saturate should no be necessary here
        #else//方法2，默认走此方法
            half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact
        #endif
        
        //剩余的点积计算 saturate限制非负范围。
        float nl = saturate(dot(normal, light.dir));
        float nh = saturate(dot(normal, halfDir));
        half lv = saturate(dot(light.dir, viewDir));
        half lh = saturate(dot(light.dir, halfDir));
        // Diffuse term//迪士尼BRDF漫反射系数计算
        half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;
        // HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
        // BUT 1) that will make shader look significantly darker than Legacy ones
        // and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
        //在Unity中，因为和旧效果适配和非重要灯光的一些原因，所以在Diffuse层面没有根据迪士尼BRDF的Diffuse部分公式一样除以Pi；
        
        // Specular term
        float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);//获得1-smooth（即Roughness）的平方，即科学意义上的roughness
        //为什么要执行这一步？答：实际上在数学公式计算的roughness（也就是公式中的α）才是真的粗糙度。但直接调整这个粗糙度，视觉感受不够线性。
        //所以为了使用户（美术工作者）能更直观地调整粗糙度，使用视觉感受更加线性的PerceptualRoughness（直觉粗糙度）来暴露给用户更加合理。
        #if UNITY_BRDF_GGX//默认会直接定义这个关键字
            // GGX with roughtness to 0 would mean no specular at all, using max(roughness, 0.002) here to match HDrenderloop roughtness remapping.
            roughness = max(roughness, 0.002);//限制roughness不为0（避免高光反射完全消失）。
            float V = SmithJointGGXVisibilityTerm(nl, nv, roughness);
            float D = GGXTerm(nh, roughness);
        #else
            // Legacy//旧版本保留而已，不会被用到，除非注释掉UnityStandardConfig.cginc中关于UNITY_BRDF_GGX的定义
            half V = SmithBeckmannVisibilityTerm(nl, nv, roughness);
            half D = NDFBlinnPhongNormalizedTerm(nh, PerceptualRoughnessToSpecPower(perceptualRoughness));
        #endif
        float specularTerm = V * D * UNITY_PI; // Torrance-Sparrow model, Fresnel is applied later
        //Cook-Torrance的反射部分。菲涅尔项后面再处理。因为Diffuse项没有除Pi，所以这里乘Pi以保证比例相等（配平）。
        #ifdef UNITY_COLORSPACE_GAMMA//Gamma空间需要开方的原理是？
            specularTerm = sqrt(max(1e-4h, specularTerm));
        #endif
        // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
        specularTerm = max(0, specularTerm * nl);// 在渲染方程中，高光项最终要乘以dot（n,l）。使用max方法避免负数。
        #if defined(_SPECULARHIGHLIGHTS_OFF)
            specularTerm = 0.0;// 材质面板中的specular Highlight开关
        #endif
        
        // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
        half surfaceReduction;
        //可能是经过观测，在物体的粗糙度越来越强时，反射会相对减弱，所以在这里假如一个根据粗糙度减弱反射的变量。（roughness = 1，reflect = 0.5）
        //因为roughnes是通过smoothness贴图采样得到，所以分线性和非线性进行不同的处理。
        #ifdef UNITY_COLORSPACE_GAMMA
            surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;      // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
        #else
            surfaceReduction = 1.0 / (roughness * roughness + 1.0);           // fade \in [0.5;1]
        #endif
        
        // To provide true Lambert lighting, we need to be able to kill specular completely.
        //假如没有镜面高光颜色，则要直接去掉高光项。Metallic =  1,albeo = 0的情况。相关讨论链接：
        //https://forum.unity.com/threads/source-of-surfacereduction-and-grazingterm-formulas-in-lighting-hlsl.714869/
        specularTerm *= any(specColor) ? 1.0: 0.0;
        half grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
        //掠射角度的反射颜色，估计也是观测值。candycat：这个half值到了FresnelLerp后会转换为灰度颜色half3。
        half3 color = diffColor * (gi.diffuse + light.color * diffuseTerm)
        + specularTerm * light.color * FresnelTerm(specColor, lh)
        + surfaceReduction * gi.specular * FresnelLerp(specColor, grazingTerm, nv);
        //gi.specular是采样反射探针的结果（已经算过roughness）。这段主要是处理的衰减变化。
        fixed4 test = fixed4(1, 1, 1, 1);
        // test.rgb = specularTerm * light.color * FresnelTerm (specColor, lh);
        // test.rgb = specColor, lh;
        // test.rgb = test.rrr;
        // test.r = grazingTerm;
        // test.rgb = surfaceReduction * gi.specular * FresnelLerp (specColor, grazingTerm, nv);
        // test.rgb = GammaToLinearSpace(test.rgb);
        // return test;
        return half4(color, 1);
    }
    
    
    inline half4 LightingStandard1(SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
    {
        
        s.Normal = normalize(s.Normal);// 法线归一化
        
        half oneMinusReflectivity;// 漫反射系数（Albedo中参与漫反射的比例）
        half3 specColor;
        s.Albedo = DiffuseAndSpecularFromMetallic(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);
        
        // shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
        // this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
        // 计算只影响Diffuse的a值
        half outputAlpha;
        s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);
        
        
        
        half4 c = BRDF1_Unity_PBS1(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
        c.a = outputAlpha;
        
        return c;
    }
    
    
    
    
#endif