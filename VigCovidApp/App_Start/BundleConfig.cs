using System.Web.Optimization;

namespace VigCovidApp
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        //"~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-3.4.1.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/Scripts").Include(
                   "~/Scripts/nifty.min.js",
                   "~/Scripts/utils.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-select.min.js",
                      "~/Scripts/bootstrapValidator.min.js",
                      "~/Scripts/bootbox.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/footable.all.min.js",
                      "~/Scripts/jquery.maskedinput.min.js",
                      "~/Scripts/bootstrap-datepicker.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/nifty.min.css",
                      "~/Content/nifty-demo-icons.min.css",
                      "~/Content/bootstrap-select.min.css",
                      "~/Content/bootstrapValidator.min.css",
                      "~/Content/css-loaders.css",
                      "~/Content/load6.css",
                      "~/Content/footable.all.min.css",
                      "~/Content/bootstrap-datepicker.min.css",
                      "~/Content/site.css"));
        }
    }
}