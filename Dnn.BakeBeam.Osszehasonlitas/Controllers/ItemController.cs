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
        public ActionResult Index()
        {
            var termekek = "";

            var ctx = DataContext.Instance();
            var osszehasonlitando = ctx.GetRepository<ProductComparison>()
                .GetById(1);

            Console.WriteLine("This is an API Sample Program for Hotcakes");
            Console.WriteLine();

            var url = string.Empty;
            var key = string.Empty;

            

            if (url == string.Empty) url = "http://www.dnndev.me";
            if (key == string.Empty) key = "1-b8bbb6d3-05f5-49eb-a95f-e81798ee9b24";

            var proxy = new Api(url, key);

            var snaps = proxy.CategoriesFindAll();
            if (snaps.Content != null)
            {
                //Console.WriteLine("Found " + snaps.Content.Count + " categories");
                termekek = "Found " + snaps.Content.Count + " categories";
               /* Console.WriteLine("-- First 5 --");
                for (var i = 0; i < 5; i++)
                {
                    if (i < snaps.Content.Count)
                    {
                        Console.WriteLine(i + ") " + snaps.Content[i].Name + " [" + snaps.Content[i].Bvin + "]");
                        var cat = proxy.CategoriesFind(snaps.Content[i].Bvin);
                        if (cat.Errors.Count > 0)
                        {
                            foreach (var err in cat.Errors)
                            {
                                Console.WriteLine("ERROR: " + err.Code + " " + err.Description);
                            }
                        }
                        else
                        {
                            Console.WriteLine("By Bvin: " + cat.Content.Name + " | " + cat.Content.Description);
                        }

                        var catSlug = proxy.CategoriesFindBySlug(snaps.Content[i].RewriteUrl);
                        if (catSlug.Errors.Count > 0)
                        {
                            foreach (var err in catSlug.Errors)
                            {
                                Console.WriteLine("ERROR: " + err.Code + " " + err.Description);
                            }
                        }
                        else
                        {
                            Console.WriteLine("By Slug: " + cat.Content.Name + " | " + cat.Content.Description);
                        }
                    }
                }*/
            }

            //Console.WriteLine("Done - Press a key to continue");
            //Console.ReadKey();



            /*try
            {
                string HotCakesApiKey = "asfaf";
                FoglalasokManager foglalasokManager = new FoglalasokManager();
                var result = foglalasokManager.FoglalasKeszites(SzemelyiEdzoID, Nev, Sport, Idopont, Megjegyzes, HotCakesApiKey);
            }
            catch (Exception ex)
            {
                return null;
            }*/



            var termekek2 = osszehasonlitando.UserId;
            ViewBag.Termekek = termekek;
            return View();
        }


        /*private void UserControlKeszlet_Load(object sender, EventArgs e)
        {
            Api proxy = apiHivas();

            //termékek betöltése datagridviewba
            var response_product = proxy.ProductsFindAll();
            var beszallitok = proxy.ManufacturerFindAll();

            if (response_product == null || response_product.Content == null || response_product.Content.Count == 0)
            {
                MessageBox.Show("Nem sikerült lekérni a termékeket vagy nincs adat.");
                return;
            }
            else
            {

                for (int i = 0; i < response_product.Content.Count; i++)
                {
                    Termek termek = new Termek();
                    termek.Név = response_product.Content[i].ProductName;
                    termek.BeszerzésiÁr = response_product.Content[i].SiteCost;

                    var keszlet = proxy.ProductInventoryFindForProduct(response_product.Content[i].Bvin);
                    termek.Raktáron = keszlet.Content[0].QuantityOnHand;
                    termek.MinimálisMennyiség = keszlet.Content[0].LowStockPoint;


                    if (keszlet.Content[0].LowStockPoint == 1)
                    {
                        termek.OptimálisMennyiség = 3;
                    }
                    else if (keszlet.Content[0].LowStockPoint == 5)
                    {
                        termek.OptimálisMennyiség = 10;
                    }
                    else if (keszlet.Content[0].LowStockPoint == 15)
                    {
                        termek.OptimálisMennyiség = 30;
                    }
                    else if (keszlet.Content[0].LowStockPoint == 50)
                    {
                        termek.OptimálisMennyiség = 100;
                    }
                    else
                    {
                        termek.OptimálisMennyiség = 300;
                    }


                    if (termek.OptimálisMennyiség > termek.Raktáron)
                    {
                        termek.OptimálishozSzükségesFt = (termek.OptimálisMennyiség - termek.Raktáron) * termek.BeszerzésiÁr;
                        termek.OptimálishozSzükségesDb = termek.OptimálisMennyiség - termek.Raktáron;
                    }
                    else
                    {
                        termek.OptimálishozSzükségesFt = 0;
                        termek.OptimálishozSzükségesDb = 0;
                    }

                    if (response_product.Content[i].ManufacturerId == "d579958c-9637-4680-958a-171f5ef37452")
                    {
                        termek.Beszállító = "BakeBeam Grillsütőgyártó kft.";
                    }
                    else if (response_product.Content[i].ManufacturerId == "067ff943-bb21-4f5f-bfec-1b66124df77e")
                    {
                        termek.Beszállító = "BakeBeam Mikrógyártó kft.";
                    }
                    else if (response_product.Content[i].ManufacturerId == "8e3d70b5-8050-4958-81aa-1f2f417d630e")
                    {
                        termek.Beszállító = "BakeBeam Kellékgyártó kft.";
                    }
                    else if (response_product.Content[i].ManufacturerId == "25afa805-3277-4272-8bfc-c5531289239b")
                    {
                        termek.Beszállító = "BakeBeam Airfryergyártó kft.";
                    }
                    else
                    {
                        termek.Beszállító = "BakeBeam Sütőgyártó kft.";
                    }

                    termekek.Add(termek);

                }

                dataGridView1.DataSource = termekek;

                //beszállítók betöltése listboxba


                for (int i = 0; i < beszallitok.Content.Count; i++)
                {
                    listBoxBeszallitok.Items.Add(beszallitok.Content[i].DisplayName);

                }

            }
        }*/
    }
}
