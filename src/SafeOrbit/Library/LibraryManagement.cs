﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeOrbit.Library.StartEarly;
using SafeOrbit.Memory;
using SafeOrbit.Memory.Injection;
using SafeOrbit.Memory.InjectionServices;

namespace SafeOrbit.Library
{
    /// <summary>
    ///     Singleton class to access core utility methods.
    /// </summary>
    public class LibraryManagement
    {
        public const SafeContainerProtectionMode DefaultInnerFactoryProtectionMode =
            SafeContainerProtectionMode.FullProtection;

        public static EventHandler<IInjectionMessage> LibraryInjected;
     
        public static void EnableInjectionProtection(InjectionAlertChannel channel)
        {
            if (Factory.CurrentProtectionMode != SafeContainerProtectionMode.FullProtection)
            {
                Factory.SetProtectionMode(SafeContainerProtectionMode.FullProtection);
            }
        }

        public static void DisableInjectionProtection()
        {
            if (Factory.CurrentProtectionMode != SafeContainerProtectionMode.NonProtection)
            {
                Factory.SetProtectionMode(SafeContainerProtectionMode.NonProtection);
            }
        }


        /// <summary>
        ///     Static holder for <see cref="Factory" />
        /// </summary>
        private static readonly Lazy<ISafeContainer> FactoryLazy = new Lazy<ISafeContainer>(SetupFactory);

        /// <summary>
        ///     Use static <see cref="Factory" /> property instead.
        /// </summary>
        internal LibraryManagement()
        {
        }

        /// <summary>
        ///     Gets the factory thread safe.
        /// </summary>
        /// <value>Factory for the assembly</value>
        public static ISafeContainer Factory => FactoryLazy.Value;

        public static SafeContainerProtectionMode ProtectionMode
        {
            get { return Factory.CurrentProtectionMode; }
            set
            {
                if (value != Factory.CurrentProtectionMode) Factory.SetProtectionMode(value);
            }
        }

        public static void StartEarly(
            SafeContainerProtectionMode protectionMode = SafeContainerProtectionMode.NonProtection)
        {
            var tasks = GetAllStartEarlyTasks(); //get all tasks
            var actions = tasks.Select(t => new Action(t.Prepare)).ToArray(); //convert them into actions
            Parallel.Invoke(actions); //run them in parallel
        }

        private static ISafeContainer SetupFactory()
        {
            var result = new SafeContainer(DefaultInnerFactoryProtectionMode);
            FactoryBootstrapper.Bootstrap(result);
            result.Verify();
            return result;
        }

        private static IEnumerable<IStartEarlyTask> GetAllStartEarlyTasks()
        {
            yield return new SafeByteFactoryInitializer(Factory); //initializes Factory as well.
            yield return new StartFillingEntropyPoolsStartEarlyTask();
        }
    }
}