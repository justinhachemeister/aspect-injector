﻿using AspectInjector.Core.Contracts;
using AspectInjector.Core.Models;
using Mono.Cecil;
using System.Collections.Generic;
using static AspectInjector.Broker.Advice;

namespace AspectInjector.Core.Advice.Effects
{
    public abstract class AdviceEffectBase : Effect
    {
        public Target Target { get; set; }

        public MethodDefinition Method { get; set; }

        public List<AdviceArgument> Arguments { get; set; } = new List<AdviceArgument>();

        public override bool IsApplicableFor(ICustomAttributeProvider target)
        {
            if ((Target & Target.Method) != 0)
                return target is MethodDefinition && !((MethodDefinition)target).IsConstructor;

            if ((Target & Target.Constructor) != 0)
                return target is MethodDefinition && ((MethodDefinition)target).IsConstructor;

            if ((Target & Target.Setter) != 0)
                return target is PropertyDefinition;

            if ((Target & Target.Getter) != 0)
                return target is PropertyDefinition;

            if ((Target & Target.EventAdd) != 0)
                return target is EventDefinition;

            if ((Target & Target.EventRemove) != 0)
                return target is EventDefinition;

            return false;
        }

        protected override bool IsEqualTo(Effect effect)
        {
            if (effect.GetType() != GetType())
                return false;

            var other = effect as AdviceEffectBase;

            return other.Target == Target && other.Method == Method;
        }

        public override bool Validate(AspectDefinition aspect, ILogger log)
        {
            if (Method.IsStatic)
            {
                log.LogError(CompilationMessage.From($"Advice {Method.FullName} cannot be static.", aspect.Host));
                return false;
            }

            if (!Method.IsPublic)
            {
                log.LogError(CompilationMessage.From($"Advice {Method.FullName} should be public.", aspect.Host));
                return false;
            }

            return true;
        }
    }
}