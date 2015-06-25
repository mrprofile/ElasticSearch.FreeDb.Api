using System.Collections.Generic;
using ElasticSearchFreeDb.ServiceModel.Types;
using Funq;
using ServiceStack;
using ElasticSearchFreeDb.ServiceInterface;

namespace ElasticSearchFreeDb
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("ElasticSearchFreeDb", typeof(FreeDbServices).Assembly)
        {
            ServiceStack.Text.JsConfig<PageResult<List<Disk>>>.EmitCamelCaseNames = true;
            ServiceStack.Text.JsConfig<Disk>.EmitCamelCaseNames = true;
            ServiceStack.Text.JsConfig<AutoComplete>.EmitCamelCaseNames = true;
        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());
        }
    }
}
