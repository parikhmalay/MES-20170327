using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.SchedulerService.Common
{
    public abstract class AbstractSchedulerHelper
    {
        /// <summary>
        /// Abstract Startup class for Unit-Tests
        /// </summary>

        private static bool HasStarted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStartup"/> class.
        /// </summary>
        /// <param name="mapsAssemblies">The maps assemblies.</param>
        /// <param name="ModelTestDataInitializer">The model test data initializer.</param>
        public AbstractSchedulerHelper(string[] mapsAssemblies)
        {
            if (!HasStarted)
            {
                for (int i = 0; i < mapsAssemblies.Length; i++)
                    MapLoader.LoadMaps(mapsAssemblies[i]); //Load Mappings
                InitKernel();

                HasStarted = true;
            }
        }

        public abstract void InitKernel();


    }
}
