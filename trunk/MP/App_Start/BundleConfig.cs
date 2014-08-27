﻿using System.Web;
using System.Web.Optimization;

namespace MP
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/kendo.all.min.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/jspdf").Include(
                      "~/Scripts/jspdf.js",
                      "~/Scripts/jspdf.plugin.standard_fonts_metrics.js",
                      "~/Scripts/jspdf.plugin.split_text_to_size.js",
                      "~/Scripts/jspdf.plugin.from_html.js",
                      "~/Scripts/jspdf.plugin.addimage.js",
                      "~/Scripts/FileSaver.min.js",
                      "~/Scripts/html2canvas.js",
                      "~/Scripts/jquery.print-area.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap.custom.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/kendo.common-bootstrap.min.css",
                      "~/Content/kendo.bootstrap.min.css",
                      "~/Content/site.css"));
        }
    }
}
