using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using beplusService.DataObjects;
using beplusService.Models;
using AutoMapper;

namespace beplusService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            // Set default and null value handling to "Include" for Json Serializer
            config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<BepDonorDTO, BepDonor>();
                cfg.CreateMap<BepDonor, BepDonorDTO>()
                    .ForMember(dst => dst.OrgId, map => map.MapFrom(x => x.BepOrganization.Id))
                    .ForMember(dst => dst.OrgName, map => map.MapFrom(x => x.BepOrganization.Name))
                    .ForMember(dst => dst.OrgAbout, map => map.MapFrom(x => x.BepOrganization.About))
                    .ForMember(dst => dst.OrgLocality, map => map.MapFrom(x => x.BepOrganization.Locality))
                    .ForMember(dst => dst.OrgEmail, map => map.MapFrom(x => x.BepOrganization.Email))
                    .ForMember(dst => dst.OrgPhone, map => map.MapFrom(x => x.BepOrganization.Phone))
                    .ForMember(dst => dst.OrgImgurl, map => map.MapFrom(x => x.BepOrganization.Imgurl));

            });

            Database.SetInitializer(new beplusInitializer());
        }
    }

    
    public class beplusInitializer : ClearDatabaseSchemaIfModelChanges<beplusContext>
    {
        protected override void Seed(beplusContext context)
        {
            /*
            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }
            */
            base.Seed(context);
        }
    }
    
}

