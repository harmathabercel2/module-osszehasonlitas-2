/*
' Copyright (c) 2025 NiBeSzoCsa
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using Dnn.BakeBeam.Osszehasonlitas.Components;
using Dnn.BakeBeam.Osszehasonlitas.Models;
using DotNetNuke.Data;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Hotcakes.CommerceDTO.v1.Client;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;


namespace Dnn.BakeBeam.Dnn.BakeBeam.Osszehasonlitas.Controllers
{
    [DnnHandleError]
    public class ItemController : DnnController
    {

        public ActionResult Delete(int itemId)
        {
            ItemManager.Instance.DeleteItem(itemId, ModuleContext.ModuleId);
            return RedirectToDefaultRoute();
        }

        public ActionResult Edit(int itemId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            var userlist = UserController.GetUsers(PortalSettings.PortalId);
            var users = from user in userlist.Cast<UserInfo>().ToList()
                        select new SelectListItem { Text = user.DisplayName, Value = user.UserID.ToString() };

            ViewBag.Users = users;

            var item = (itemId == -1)
                 ? new Item { ModuleId = ModuleContext.ModuleId }
                 : ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);

            return View(item);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(Item item)
        {
            if (item.ItemId == -1)
            {
                item.CreatedByUserId = User.UserID;
                item.CreatedOnDate = DateTime.UtcNow;
                item.LastModifiedByUserId = User.UserID;
                item.LastModifiedOnDate = DateTime.UtcNow;

                ItemManager.Instance.CreateItem(item);
            }
            else
            {
                var existingItem = ItemManager.Instance.GetItem(item.ItemId, item.ModuleId);
                existingItem.LastModifiedByUserId = User.UserID;
                existingItem.LastModifiedOnDate = DateTime.UtcNow;
                existingItem.ItemName = item.ItemName;
                existingItem.ItemDescription = item.ItemDescription;
                existingItem.AssignedUserId = item.AssignedUserId;

                ItemManager.Instance.UpdateItem(existingItem);
            }

            return RedirectToDefaultRoute();
        }

        

       


        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index(string hozzaadottTermekSKU)
        {

            var ctx = DataContext.Instance();
            var adatbazisObjektum = ctx.GetRepository<ProductComparison>().Find("where UserId = @0", User.UserID);
            ProductComparison osszehasonlitando;

            if (adatbazisObjektum.ToArray().Length != 0)
            {
                osszehasonlitando = adatbazisObjektum.First();
            }
            else
            {
                ProductComparison ujFelhasznalo = new ProductComparison();
                ujFelhasznalo.CreatedUtc = DateTime.UtcNow;
                ujFelhasznalo.UserId = User.UserID;
                ctx.GetRepository<ProductComparison>().Insert(ujFelhasznalo);

                osszehasonlitando = ujFelhasznalo;
            }

            if (hozzaadottTermekSKU != null){
                

                ProductComparisonItem ujElem = new ProductComparisonItem();
                ujElem.AddedUtc = DateTime.UtcNow;
                ujElem.ProductBvin = hozzaadottTermekSKU;
                ujElem.ComparisonId = osszehasonlitando.Id;

                ctx.GetRepository<ProductComparisonItem>().Insert(ujElem);
            }


            var osszehasonlitandoElemek = ctx.GetRepository<ProductComparisonItem>().Find("where ComparisonId = @0", osszehasonlitando.Id).ToArray();


            var url = string.Empty;
            var key = string.Empty;



//            if (url == string.Empty) url = "http://www.dnndev.me";
//            if (key == string.Empty) key = "1-b8bbb6d3-05f5-49eb-a95f-e81798ee9b24";

            if (url == string.Empty) url = "http://rendfejl1013.northeurope.cloudapp.azure.com/";
            if (key == string.Empty) key = "1-2ee33390-04c3-4972-96df-d8ff7ef2bc07";

            var proxy = new Api(url, key);

            var snaps = proxy.CategoriesFindAll();


            Array.Sort(osszehasonlitandoElemek, delegate (ProductComparisonItem a, ProductComparisonItem b)
            {
                return b.AddedUtc.CompareTo(a.AddedUtc); // csökkenő sorrend
            });



            ViewBag.ElemSzam = osszehasonlitandoElemek.Length;

            List<TermekAdat> termekTulajdonsagok = new List<TermekAdat>();

            int osszehasonlitandokSzama = 5;

            if(osszehasonlitandoElemek.Length < 5)
            {
                osszehasonlitandokSzama = osszehasonlitandoElemek.Length;
            }

            for(int a = 0; a < osszehasonlitandokSzama; a++)
            {
                var termek = proxy.ProductsFindBySku(osszehasonlitandoElemek[a].ProductBvin.ToString());

                String termekBvin = termek.Content.Bvin;

                TermekAdat adatTarto = new TermekAdat();

                adatTarto.Termek1Sku = termek.Content.Sku;
                adatTarto.Termek1Bvin = termekBvin;
                adatTarto.Termek1N = termek.Content.ProductName.ToString();
                adatTarto.Termek1P = tizedesJegyLevetel(termek.Content.SitePrice.ToString());
                adatTarto.Termek1W = tizedesJegyLevetel(termek.Content.ShippingDetails.Weight.ToString());
                adatTarto.Termek1Kep = termek.Content.ImageFileMedium.ToString();
                adatTarto.Termek1Meret = tizedesJegyLevetel(termek.Content.ShippingDetails.Length.ToString()) + " cm x "
                                + tizedesJegyLevetel(termek.Content.ShippingDetails.Width.ToString()) + " cm x "
                                + tizedesJegyLevetel(termek.Content.ShippingDetails.Height.ToString()) + " cm";


                List<string> egyediTulajdonsagok = new List<string>();
                try
                {
                    var tulajd = proxy.ProductPropertiesForProduct(termekBvin).Content.ToArray();

                    for (int i = 0; i < tulajd.Length; i++)
                    {
                        ProductPropertyValue ertek = ctx.GetRepository<ProductPropertyValue>().Find("where ProductBvin = @0 and PropertyId = @1", termekBvin, tulajd[i].Id).First();
                        egyediTulajdonsagok.Add(tulajd[i].DisplayName + ": " + ertek.PropertyValue);
                    }
                } catch(Exception ex) { }
               
                adatTarto.egyediTulajdonsagok = egyediTulajdonsagok;

                termekTulajdonsagok.Add(adatTarto);
            }

            ViewBag.termekTulajdonsagok = termekTulajdonsagok;

            return View();
        }


        public string tizedesJegyLevetel(string nyers)
        {
            decimal value = decimal.Parse(nyers);
            string formatted = value.ToString("0.##");
            return formatted;
        }

    }
}
