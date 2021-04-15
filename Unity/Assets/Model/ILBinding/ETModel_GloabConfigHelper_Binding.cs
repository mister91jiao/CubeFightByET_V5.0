using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class ETModel_GloabConfigHelper_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.GloabConfigHelper);

            field = type.GetField("tick", flag);
            app.RegisterCLRFieldGetter(field, get_tick_0);
            app.RegisterCLRFieldSetter(field, set_tick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_tick_0, AssignFromStack_tick_0);
            field = type.GetField("controllerType", flag);
            app.RegisterCLRFieldGetter(field, get_controllerType_1);
            app.RegisterCLRFieldSetter(field, set_controllerType_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_controllerType_1, AssignFromStack_controllerType_1);


        }



        static object get_tick_0(ref object o)
        {
            return ETModel.GloabConfigHelper.tick;
        }

        static StackObject* CopyToStack_tick_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.GloabConfigHelper.tick;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_tick_0(ref object o, object v)
        {
            ETModel.GloabConfigHelper.tick = (System.Single)v;
        }

        static StackObject* AssignFromStack_tick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @tick = *(float*)&ptr_of_this_method->Value;
            ETModel.GloabConfigHelper.tick = @tick;
            return ptr_of_this_method;
        }

        static object get_controllerType_1(ref object o)
        {
            return ETModel.GloabConfigHelper.controllerType;
        }

        static StackObject* CopyToStack_controllerType_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ETModel.GloabConfigHelper.controllerType;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_controllerType_1(ref object o, object v)
        {
            ETModel.GloabConfigHelper.controllerType = (ETModel.ControllerType)v;
        }

        static StackObject* AssignFromStack_controllerType_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ETModel.ControllerType @controllerType = (ETModel.ControllerType)typeof(ETModel.ControllerType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ETModel.GloabConfigHelper.controllerType = @controllerType;
            return ptr_of_this_method;
        }



    }
}
