#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// The format all message dispatchers must follow.
    /// <br />
    /// A message dispatcher is incharge of mapping methods to valid routes that will be invoked when a client requests to communicate to a specific route.
    /// </summary>
    /// <typeparam name="TPayloadType">The type which messages will be serialized to.</typeparam>
    public abstract class Dispatcher<TPayloadType> where TPayloadType : class, new() 
    {
        /// <summary>
        /// List of XPath expressions to evaluate. If the expression is valid execute the target method. 
        /// <br />
        /// We assume the method would always return null (the method passed is void) or an XML Document.
        /// </summary>
        private readonly List<(RouteAttribute route, Func<TPayloadType, Task<TPayloadType?>> targetMethod)> _handlers = new List<(RouteAttribute route, Func<TPayloadType, Task<TPayloadType?>> targetMethod)>();

        protected abstract bool IsValidMatch(RouteAttribute route, TPayloadType message);
        protected abstract RouteAttribute? GetAttribute(MethodInfo methodInfo);
        protected abstract TParam Deserialize<TParam>(TPayloadType payload);
        protected abstract object Deserialize(Type parameterType, TPayloadType payload);
        protected abstract TPayloadType? Serialize<TResult>(TResult result);

        protected bool HasAttribute(MethodInfo methodInfo)
        {
            return GetAttribute(methodInfo) != null;
        }

        /// <summary>
        /// Registers the target methods to be called via its route attribute.
        /// <br />
        /// The target parameters initially passed in a serialized format must be deserialized in order to be properly passed to the target method.
        /// <br />
        /// The result of the target method must be reserialized into a serialized format to be retruned to the method caller.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameters to pass to the target method.</typeparam>
        /// <typeparam name="TResult">The type of the returned output of the target method.</typeparam>
        /// <param name="target">The target method to regiter to a route.</param>
        public virtual void Register<TParam, TResult>(Func<TParam, Task<TResult>> target)
        {
            if (!HasAttribute(target.Method))
            {
                throw new Exception("No RouteAttribute was specified.");
            }

            Func<TPayloadType, Task<TPayloadType?>> wrapper = new Func<TPayloadType, Task<TPayloadType?>>(
            async payload =>
            {
                TParam param = Deserialize<TParam>(payload);
                TResult result = await target(param);

                if (result != null)
                {
                    return Serialize(result);
                }
                else
                {
                    return null;
                }
            });

            AddHandler(GetAttribute(target.Method), wrapper);
        }

        /// <summary>
        /// Registers the target methods to be called via its route attribute.
        /// <br />
        /// The target parameters initially passed in a serialized format must be deserialized in order to be properly passed to the target method.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameters to pass to the target method.</typeparam>
        /// <param name="target">The target method to regiter to a route.</param>
        public virtual void Register<TParam>(Func<TParam, Task> target)
        {
            if (!HasAttribute(target.Method))
            {
                throw new Exception("No RouteAttribute was specified.");
            }

            Func<TPayloadType, Task<TPayloadType?>> wrapper = new Func<TPayloadType, Task<TPayloadType?>>(
            async payload =>
            {
                TParam param = Deserialize<TParam>(payload);
                await target(param);
                return null;
            });

            AddHandler(GetAttribute(target.Method), wrapper);
        }

        protected void AddHandler(RouteAttribute route, Func<TPayloadType, Task<TPayloadType?>> callback)
        {
            _handlers.Add((route, callback));
        }

        /// <summary>
        /// Evaluates the handlers. If valid, pass the serialized document to the target method and execute it.
        /// </summary>
        /// <param name="message">The XML Document to pass to the target method.</param>
        /// <returns>The value returned by the target method.</returns>
        public async Task<TPayloadType?> DispatchAsync(TPayloadType message)
        {
            foreach (var (route, target) in _handlers)
            {
                if (IsValidMatch(route, message))
                {
                    return await target(message);
                }
            }
            //No handler?? what to do??
            return null;
        }

        public void Bind<TProtocol>(NetworkChannel<TProtocol, TPayloadType> networkChannel) where TProtocol : Protocol<TPayloadType>, new()
        {
            networkChannel.SetCallBack(
                async message =>
                {
                    TPayloadType? response = await DispatchAsync(message).ConfigureAwait(false);

                    if (response != null)
                    {
                        try
                        {
                            await networkChannel.SendAsync(response).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            );
        }

        /// <summary>
        /// WARNING: May not be fit for a production environment due to heavy reliance on reflection.
        /// TODO - Consider using expression trees.
        /// </summary>
        public void Bind<THandler>()
        {
            static bool assertTypeIsTask(MethodInfo methodInfo)
            {
                return methodInfo.ReturnType.IsAssignableFrom(typeof(Task));
            }

            static bool assertTypeIsTaskT(MethodInfo methodInfo)
            {
                return methodInfo.ReturnType.BaseType?.IsAssignableFrom(typeof(Task)) ?? false;

            }

            List<MethodInfo> taskList = typeof(THandler).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                        .Where(HasAttribute)
                                                        .Where(task => task.GetParameters().Count() == 1 && (assertTypeIsTask(task) || assertTypeIsTaskT(task)))
                                                        .ToList();

            foreach (MethodInfo method in taskList)
            {
                var wrapper = new Func<TPayloadType, Task<TPayloadType?>>(
                    async payload =>
                    {
                        var @param = Deserialize(method.GetParameters()[0].ParameterType, payload);

                        try
                        {
                            if (assertTypeIsTask(method))
                            {
                                var task = method.Invoke(null, new object[] { param }) as Task;

                                if (task != null)
                                {
                                    await task;
                                }

                                return null;
                            }
                            else
                            {
                                var taskResult = (await (method.Invoke(null, new object[] { param }) as dynamic) as dynamic);

                                if (taskResult != null)
                                {
                                    return Serialize(taskResult as dynamic);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            return null;
                        }
                    }
                );

                AddHandler(GetAttribute(method), wrapper);
            }
        }
    }
}
