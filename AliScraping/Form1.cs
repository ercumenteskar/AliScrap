using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using My;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Threading;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using Knapcode.TorSharp;
using System.Net.Http;
using System.Net.Sockets;
using Extreme.Net;

namespace WindowsFormsApplication2
{
  public partial class Form1 : Form
  {
    AliModel db = new AliModel();
    ChromeDriverService service = null;
    String itemsep1 = "¶"; // ASCII : 20
    //String itemsep2 = "§"; // ASCII : 21
    String LastStore = "";
    int activeVpnNo = 0;
    //List<String> listvarvals = new List<String>();
    List<DateTime> vpntimes = new List<DateTime>();
    Socket torserver = null;
    public Form1()
    {
      InitializeComponent();
    }

    //private PhantomJSDriver _driver;
    private ChromeDriver _driver;
    public ChromeDriver driver //PhantomJSDriver driver
    {
      get
      {
        //if (service == null)
        //  service = ChromeDriverService.CreateDefaultService();
        //if (_driver == null)
        //{
        //  service.HideCommandPromptWindow = false;
        //  /*
        //  PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
        //  service.IgnoreSslErrors = true;
        //  service.LoadImages = false;
        //  service.ProxyType = "none";
        //  service.SslProtocol = "tlsv1"; //"any" also works
        //  */
        //  //"C:\Users\Erci\AppData\Local\Google\Chrome\User Data\Default"
        //  ChromeOptions options = new ChromeOptions();
        //  options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
        //  //options.AddArguments("user-data-dir=C:\\Users\\Erci\\AppData\\Local\\Google\\Chrome\\User Data\\Default");
        //  //options.AddArguments("user-data-dir=C:\\Users\\Administrator\\AppData\\Local\\Google\\Chrome\\User");
        //  String mstr = tb_mod.Text;
        //  if (mstr != "")
        //  {
        //    //            (avpn % vpncnt) * modcnt + mod
        //    mstr = (Int32.Parse(mstr) + (Int32.Parse(tb_modcount.Text) * (activeVpnNo % Int32.Parse(tb_vpncount.Text)))).ToString();
        //    while (vpntimes.Count < Int32.Parse(mstr))
        //    {
        //      vpntimes.Add(DateTime.Now.AddSeconds(-30));
        //    }
        //    var dt1 = vpntimes[Int32.Parse(mstr) - 1].AddSeconds(30);
        //    while (dt1 > DateTime.Now)
        //      Thread.Sleep(1000);
        //    var tmpdt = vpntimes[Int32.Parse(mstr) - 1];
        //    vpntimes[Int32.Parse(mstr) - 1] = DateTime.Now;
        //    //this.Invoke((MethodInvoker)delegate
        //    //{
        //    //  textBox1.Text += "\r\n" + mstr + " : " + vpntimes[Int32.Parse(mstr) - 1].ToLongTimeString() + "(" + tmpdt.ToLongTimeString() + ")";
        //    //});
        //    //            textBox1.Text += "\r\n" + mstr + " : " + vpntimes[Int32.Parse(mstr) - 1].ToLongTimeString() + "(" + tmpdt.ToLongTimeString() + ")";
        //    mstr = "\\" + mstr;
        //    activeVpnNo++;
        //  }
        //  String dir = "C:\\ChromeSettings" + mstr;
        //  if (!Directory.Exists(dir))
        //    Directory.CreateDirectory(dir);
        //  options.AddArguments("user-data-dir=" + dir);
        //  //options.AddArguments("-start-maximized");
        //  //          _driver = new ChromeDriver(service, options);
        //  _driver = new ChromeDriver(service, options);
        //}
        return _driver;
      }
    }

    private String getText(String className, String tag = "", String html = "", String attribute = "")
    {
      if (html == "")
        html = driver.PageSource;
      if (html.IndexOf(className) < 0)
        return "";
      else if (tag != "")
      {
        html = html.Substring(html.IndexOf(className));
        html = html.Substring(html.IndexOf(">") + 1);
        html = html.Substring(html.IndexOf("<" + tag) + tag.Length + 2);
        html = html.Substring(0, html.IndexOf("<"));
        return html;
      }
      else
      {
        html = html.Substring(html.IndexOf(className));
        html = html.Substring(html.IndexOf(">") + 1);
        html = html.Substring(0, html.IndexOf("<"));
        return html;
      }

    }
    private DateTime aliDT2DT(String DT)
    {
      string year = DT.Substring(DT.IndexOf(",") + 1);
      string month = DT.Substring(0, 3).ToUpper();
      if (month == "JAN") month = "1";
      else if (month == "FEB") month = "2";
      else if (month == "MAR") month = "3";
      else if (month == "APR") month = "4";
      else if (month == "MAY") month = "5";
      else if (month == "JUN") month = "6";
      else if (month == "JUL") month = "7";
      else if (month == "AUG") month = "8";
      else if (month == "SEP") month = "9";
      else if (month == "OCT") month = "10";
      else if (month == "NOV") month = "11";
      else if (month == "DEC") month = "12";
      string day = DT.Substring(DT.IndexOf(" ") + 1);
      day = day.Substring(0, day.IndexOf(","));
      var dts = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
      return dts;
    }
    private async void button1_Click(object sender, EventArgs e)
    {
      //if (_driver != null)
      //  reCreateDriver();
      //driver.Navigate().GoToUrl("about:blank");
      //Thread.Sleep(1000);
      //driver.Navigate().Back();
      //return;

      Dictionary<String, String> list = new Dictionary<string, string>();
      int pagenum = 1;
      //      int stime = 1;
      //String linkim = "";
      if ((tb_mod.Text != "") && (tb_mod.Text != "1"))
        textBox1.Lines = textBox1.Lines.Skip(Int32.Parse(tb_mod.Text) - 1).ToArray();
      while ((textBox1.Lines.Count() > 0) && (textBox1.Lines[0] != ""))
        try
        {
          //textBox1.Lines[0] + "&page=" + pagenum.ToString() = textBox1.Lines[0] + "&page=" + pagenum.ToString();//"https://www.aliexpress.com/category/100003109/women-clothing-accessories.html?spm=2114.01010108.0.0.cCRblR&isrefine=y&site=glo&SortType=total_tranpro_desc&g=y&tc=af&tag=&needQuery=n&pvId=2-201563827";
          //"https://www.aliexpress.com/category/200003482/dresses.html?isFavorite=y";
          // Tüm Kadın kategorisi : https://www.aliexpress.com/category/100003109/women-clothing-accessories.html?spm=2114.01010108.0.0.YQzmYd&isrefine=y&site=glo&g=y&SortType=total_tranpro_desc&tc=af&tag=&needQuery=n
          Navigate(textBox1.Lines[0] + "&page=" + pagenum.ToString());
          //driver.Navigate().GoToUrl(textBox1.Lines[0] + "&page=" + pagenum.ToString());
          //while (driver.Url != textBox1.Lines[0] + "&page=" + pagenum.ToString())
          //{
          //  Thread.Sleep(stime * 1000);
          //  driver.Navigate().Back();
          //  driver.Navigate().GoToUrl(textBox1.Lines[0] + "&page=" + pagenum.ToString());
          //  stime++;
          //}
          //stime = 1;
          while (!((driver.FindElementByClassName("search-count").Text == "0") || (driver.PageSource.Contains("did not match any products"))))
          {
            DateTime ft = DateTime.Now.AddSeconds(1);
            int y = 100;
            //for (int i = 0; i < 200; i++)
            var tmppgsrc = driver.PageSource;
            while (DateTime.Now < ft) //(tmppgsrc == driver.PageSource) // 
            {
              driver.ExecuteScript("window.scrollTo(0, " + y.ToString() + ")"); // (document.body.scrollHeight)
              y = y + 100;
            }
            y = 0;
            foreach (var item in driver.FindElementById("list-items").FindElement(By.TagName("ul")).FindElements(By.TagName("li")))
            {
              y++;
              bool kriterlereuygun = false;
              String satirsource = item.GetAttribute("innerHTML"); //Feedback
              int storerank = 0;
              int storerankkri = 0;
              decimal storerating = 0;
              decimal storeratingkri = 0;
              decimal starrating = 0;
              decimal ratekri = 4.5M;
              int feedback = 0;
              int feedbackkri = 20;
              int orderkri = 100;
              int order = 0;
              if (satirsource.Contains("title=\"Star Rating: "))
                decimal.TryParse(satirsource.OrtasiniGetir("title=\"Star Rating: ", " ").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), out starrating);
              if (satirsource.Contains("title=\"Feedback"))
                Int32.TryParse(satirsource.OrtasiniGetir("title=\"Feedback(", ")"), out feedback);
              if (satirsource.Contains("<em title=\"Total Orders\">"))
                Int32.TryParse(satirsource.OrtasiniGetir("<em title=\"Total Orders\">", "</em>").Replace("Orders", "").Replace(" ", "").Replace("(", "").Replace(")", ""), out order);
              if (satirsource.Contains("class=\"score-icon"))
                Int32.TryParse(satirsource.OrtasiniGetir("class=\"score-icon", ">").OrtasiniGetir("feedbackscore=\"", "\"").Replace(",", ""), out storerank);
              if (satirsource.Contains("class=\"score-icon"))
                decimal.TryParse(satirsource.OrtasiniGetir("class=\"score-icon", ">").OrtasiniGetir("sellerpositivefeedbackpercentage=\"", "\"").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), out storerating);
              kriterlereuygun = (
                (starrating >= ratekri)
                && (feedback >= feedbackkri)
                && (order >= orderkri)
                && (storerank >= storerankkri)
                && (storerating >= storeratingkri)
              );
              if ((kriterlereuygun) && (satirsource.Contains("class=\"info")))
              {
                var ele = item.FindElement(By.TagName("a"));
                var link = "http://" + ele.GetAttribute("href").OrtasiniGetir("www.", ".html", true); // başında http olmazsa driver linki açmıyor...
                var img = "";
                try
                {
                  img = ele.FindElement(By.TagName("img")).GetAttribute("src");
                  if (img.EndsWith("_220x220.jpg"))
                    img = img.Substring(0, img.Length - 12);
                }
                catch (Exception) { img = ""; };
                String pricetext = item.FindElement(By.ClassName("item")).FindElement(By.ClassName("info")).GetAttribute("innerHTML");
                pricetext = pricetext.OrtasiniGetir("<span class=\"price price-m\">", "</div>");
                pricetext = pricetext.OrtasiniGetir("<span class=\"value\" itemprop=\"price\">", "</span>");
                img = img + itemsep1 + pricetext + itemsep1 + starrating + itemsep1 + feedback + itemsep1 + order + itemsep1 + storerank + itemsep1 + storerating;
                if (!list.Keys.ToList().Exists(xx => xx == link))
                  list.Add(link, img);
              }
              this.Text = y.ToString();
            }
            for (int i = 0; i < list.Count(); i++)
            {
              String strx = list.Keys.ToList()[i].Split('/')[5];
              var prod = db.Products.Find(strx.Substring(0, strx.IndexOf(".")));
              bool gir = true;
              bool changed = false;
              if (prod != null)
              {
                //                changed = changed || (prod.Rate - decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[2]) != 0);
                prod.Rate = decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[2]);
                //                changed = changed || (prod.Last6MOrderCount - Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[3]) != 0);
                prod.Last6MOrderCount = Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[3]);
                //                changed = changed || (prod.OrderCount - Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[4]) != 0);
                prod.OrderCount = Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[4]);
                //                changed = changed || (prod.Store.Rank - Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[5]) != 0);
                prod.Store.Rank = Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[5]);
                //                changed = changed || (prod.Store.Rating - decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[6]) != 0);
                prod.Store.Rating = decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[6]);
                if (prod.PriceRange == null)
                  prod.PriceRange = list.Values.ToList()[i].Split(Char.Parse(itemsep1))[1];
                gir = (prod.PriceRange != list.Values.ToList()[i].Split(Char.Parse(itemsep1))[1]) && (list.Values.ToList()[i].Split(Char.Parse(itemsep1))[1].Contains("-")); // Fiyat değişmiş...
                changed = true;
                prod.LastCheckDT = DateTime.Now;
              }
              if (gir)
              {
                await Task.Factory.StartNew(() => getLink(list.Keys.ToList()[i], list.Values.ToList()[i]));
                driver.Navigate().Back();
                reCreateDriver();
                changed = false;
              }
              if (changed)
                db.SaveChanges();
              this.Text = LastStore + " " + pagenum.ToString() + "(" + i.ToString() + "/" + list.Count() + ")";
            }
            this.Text = LastStore + " " + pagenum + "(" + list.Count() + "/" + list.Count() + ")";
            pagenum++;
            //driver.Quit();
            //driver.Dispose();
            //_driver = null;
            list.Clear();
            driver.Navigate().Back();
            reCreateDriver();
            Navigate(textBox1.Lines[0] + "&page=" + pagenum.ToString());
            //while (driver.Url != textBox1.Lines[0] + "&page=" + pagenum.ToString())
            //{
            //  Thread.Sleep(stime * 1000);
            //  driver.Navigate().Back();
            //  driver.Navigate().GoToUrl(textBox1.Lines[0] + "&page=" + pagenum.ToString());
            //  stime++;
            //}
            //stime = 1;
          }
          textBox1.Lines = textBox1.Lines.Skip(tb_modcount.Text == "" ? 1 : Int32.Parse(tb_modcount.Text)).ToArray();
          pagenum = 1;
        }
        catch { }
      driver.Close();
      service.Dispose();
    }

    private void getLink(string link, string img, bool save = true)
    {
      String pricetext = "";
      if (img.Contains(itemsep1))
        pricetext = img.Split(Char.Parse(itemsep1))[1]; // bunu yine image e çevirelim...
      img = img.Split(Char.Parse(itemsep1))[0]; // bunu yine image e çevirelim...
      //await Task.Factory.StartNew(() => driver.Navigate().GoToUrl(link));
      Navigate(link);
      //driver.Navigate().GoToUrl(link);
      String source = driver.PageSource;
      Store store = null;
      if (!driver.PageSource.Contains("product-name")) return; // 404 not found Ör: https://www.aliexpress.com/item/Runway-Dresses-2016-Gorgeous-Gray-Long-Sleeves-Sheer-Mesh-Embroidery-Bohemian-Brand-Style-Vestidos-De-Festa/32750647700.html
      //      if (driver.PageSource.Contains("class=\"store-header-container")) // mağaza bilgileri bulundu.

      String pid = driver.FindElementById("hid-product-id").GetAttribute("value");
      Product product = db.Products.Find(pid);
      bool newitem = product == null;
      IWebElement ele = null;
      if (newitem)
      {
        product = new Product();
        db.Products.Add(product);
        product.Id = pid;
        product.Link = link.Substring(0, link.IndexOf(".html") + 5);
        product.MainImg = img;
        product.Title = driver.FindElementByClassName("product-name").Text;

        if (driver.FindElementByClassName("store-header-container") != null) // mağaza bilgileri bulundu.
        {
          store = db.Stores.Find(Int32.Parse(getText("store-number").Replace("Store No.", "")));
          if (store == null)
          {
            int rank = 0;
            decimal rating = 0;
            if (driver.PageSource.Contains("class=\"rank-num\""))
            {
              rank = Int32.Parse(driver.FindElementByXPath("//*[@class=\"rank-num\"]").Text);
              rating = decimal.Parse(driver.FindElementByXPath("//*[@class=\"positive-percent\"]").Text.Replace("%", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            }
            store = new Store
            {
              Id = Int32.Parse(getText("store-number").Replace("Store No.", "")),
              Location = getText("store-location").Replace(Environment.NewLine, "").Trim(),
              Rank = rank,
              Rating = rating,
              Title = driver.FindElementByXPath("//*[@class=\"shop-name\"]/a").Text,
              Since = aliDT2DT(getText("store-time", "em"))
            };
            db.Stores.Add(store);
          }
          LastStore = store.Title;
        }
        Category category = null;
        String cattitle;
        int catid, parentid = 0;
        String tmp = source.OrtasiniGetir("breadcrumb-home-link", "</div>"), tmp2 = "", tmp3 = "";
        while (tmp.IndexOf(".com/category") > 0)
        {
          catid = Int32.Parse(tmp.OrtasiniGetir("www.aliexpress.com/category/", "/"));
          cattitle = tmp.OrtasiniGetir("title=\"", "\">");
          category = db.Categories.Find(catid);
          if (category == null)
          {
            category = new Category
            {
              Id = catid,
              Name = cattitle,
              ParentId = parentid
            };
            db.Categories.Add(category);
          }
          parentid = catid;
          tmp = tmp.Substring(tmp.IndexOf(".com/category") + 5);
          tmp = tmp.Substring(tmp.IndexOf("</a>") + 5);
        }
        //if (store == null) throw new Exception("Mağaza bilgileri bulunamadı");
        if ((category == null) && (db.Categories.Find(0) == null))
        {
          category = new Category { Id = 0, Name = "Home" };
          db.Categories.Add(category);//throw new Exception("Kategori bulunamadı");
        }
        product.Store = store;
        product.Category = category;


        PropertyKey key = null;
        PropertyValue val = null;
        int tmpi;
        String tmps, tmps2;
        ele = driver.FindElementByXPath("//*[@id=\"j-product-desc\"]/div[1]/div[2]/ul");
        foreach (var item in ele.FindElements(By.ClassName("property-item")))
        {
          tmps = item.GetAttribute("id").Replace("product-prop-", "");
          if (tmps != "")
          {
            tmpi = Int32.Parse(tmps);
            tmps = item.FindElement(By.ClassName("propery-title")).Text;
            tmps = tmps.Substring(0, tmps.Length - 1);
            key = db.PropertyKeys.Find(tmpi);
            if (key == null)
            {
              key = new PropertyKey { Id = tmpi, Name = tmps };
              db.PropertyKeys.Add(key);
            }
            tmps = item.GetAttribute("data-attr");
            if (tmps != "")
            {
              for (int i = 0; i < tmps.Split(',').Count(); i++)
              {
                tmps2 = item.FindElement(By.ClassName("propery-des")).Text;
                val = db.PropertyValues.Find(Int32.Parse(tmps.Split(',')[i]), tmpi);
                if (val == null)
                {
                  val = new PropertyValue { Id = Int32.Parse(tmps.Split(',')[i]), KeyId = tmpi, Name = tmps2.Split(',')[i] };
                  db.PropertyValues.Add(val);
                }
                if (db.ProductProperties.Find(product.Id, key.Id, val.Id) == null)
                  db.ProductProperties.Add(new ProductProperty { Product = product, KId = key.Id, VId = val.Id });
              }
            }
          }
        }

        tmp = source.Substring(source.IndexOf("<div class=\"description-content"));
        tmp = tmp.Substring(0, tmp.IndexOf("<div class=\"ui-box pnl-packaging-main") - 1);
        int y = 50;
        tmp = "loading32";
        tmp = driver.PageSource;
        //      while (tmp == driver.PageSource)
        while (tmp.Contains("loading32"))
        {
          driver.ExecuteScript("window.scrollTo(0, " + y.ToString() + ")"); // (document.body.scrollHeight)
          tmp = driver.FindElementByClassName("description-content").GetAttribute("innerHTML");
          y = y + 100;
        }
        product.DescHtml = tmp;

        var cn = driver.FindElementsByClassName("product-packaging-list")[0];

        foreach (var item in cn.FindElements(By.TagName("li")))
        {
          if (item.FindElement(By.ClassName("packaging-title")).Text == "Unit Type:")
            tmp = item.FindElement(By.ClassName("packaging-des")).Text;
          else if (item.FindElement(By.ClassName("packaging-title")).Text == "Package Weight:")
            tmp2 = item.FindElement(By.ClassName("packaging-des")).GetAttribute("rel");
          else if (item.FindElement(By.ClassName("packaging-title")).Text == "Package Size:")
            tmp3 = item.FindElement(By.ClassName("packaging-des")).Text;
        }

        Unit unit = db.Units.FirstOrDefault(u => u.Name == tmp);
        if (unit == null)
        {
          unit = new Unit { Name = tmp };
          db.Units.Add(unit);
        }
        product.Unit = unit;

        product.Weight = decimal.Parse(tmp2.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
        product.Size = tmp3;
        product.CreateDT = DateTime.Now;

        #region images
        db.ProductImages.RemoveRange(db.ProductImages.Where(u => u.ProductId == product.Id));
        int posi = 1;
        foreach (var imgsrc in source.OrtasiniGetir("window.runParams.imageBigViewURL=[", "]").Replace("	", "").Replace("\"", "").Replace("\r\n", "").Split(','))
        {
          db.ProductImages.Add(new ProductImage { ProductId = product.Id, Position = posi, Src = imgsrc }); // , ThumbSrc = imgsrc + "_50x50.jpg" 
          posi++;
        }
        posi = 1001;
        tmps = driver.FindElement(By.ClassName("description-content")).GetAttribute("innerHTML");
        while (tmps.Contains("src=\"http"))
        {
          tmps2 = "http" + tmps.OrtasiniGetir("src=\"http", "\"");
          if (tmps2.Contains(".jpg"))
          {
            int width = 0;
            Int32.TryParse(tmps2.OrtasiniGetir("width=", "&"), out width);// 0; if ((tmps2.Contains("width")) && (tmps2.Contains("width"))) width = Int32.Parse(tmps2.OrtasiniGetir("width=", "&"));
            int height = 0; //if (tmps2.Contains("height"))
            Int32.TryParse(tmps2.OrtasiniGetir("height=", "&"), out height);
            string tmps3;
            if (((width == 0) || (height == 0)) && (tmps2.Substring(tmp2.IndexOf(".jpg") + 3).Contains(".jpg"))) // boyutlar alınamadı ise ve dosya isminde 2 kez .jpg geçiyorsa
            {
              tmps3 = tmps2.OrtasiniGetir(".jpg_", ".jpg");
              Int32.TryParse(tmps3.Substring(0, tmps3.IndexOf("x")), out width);
              Int32.TryParse(tmps3.Substring(tmps3.IndexOf("x") + 1), out height);
            }
            String hash = ""; if (tmps2.Contains("hash")) hash = tmps2.Substring(tmps2.IndexOf("hash=") + 5);
            tmps2 = tmps2.OrtasiniGetir("http", ".jpg", true);
            //<img height="640" src="https://ae01.alicdn.com/img/pb/839/733/925/925733839_432.jpg" width="640">
            //OR
            //https://ae01.alicdn.com/kf/HTB1rUSlLXXXXXc2XVXXq6xXFXXXd/222988234/HTB1rUSlLXXXXXc2XVXXq6xXFXXXd.jpg?size=61483&height=567&width=960&hash=eed561a11e6814d4c0f539a00d1c3c33
            if (hash.Length > 32)
              hash = hash.Substring(0, 32); // sonunda .jpg yazan hash gördüm...
            db.ProductImages.Add(new ProductImage { ProductId = product.Id, Position = posi, Src = tmps2, Width = width, Height = height, Hash = hash });
            posi++;
          }
          tmps = tmps.Substring(tmps.IndexOf("src=\"http") + 10);
        }

        #endregion
      }
      product.PriceRange = pricetext;
      UpdateProduct(ref product);
      #region Variants
      /*
      String pageSource = driver.PageSource;
      Dictionary<int, String> variantKeys = new Dictionary<int, String>(); // 14, Color ; 25, Size vb.
      Dictionary<int, String> variantValues1 = new Dictionary<int, String>(); // 213, Red; 3545, Blue vb.
      Dictionary<int, String> variantValues2 = new Dictionary<int, String>(); // 987, 35; 5746, 36 vb.
      Dictionary<int, String> variantValues3 = new Dictionary<int, String>(); // vb.
      var tmpvariants = dirivir.FindElementById("j-product-info-sku");
      if (tmpvariants != null)
      {
        var tmpvi = 1;
        var variants = tmpvariants.FindElements(By.ClassName("p-property-item"));
        foreach (var vkey in variants)
        {
          int keyid = Int32.Parse(vkey.FindElement(By.ClassName("p-item-main")).FindElement(By.TagName("ul")).GetAttribute("data-sku-prop-id"));
          String keyname = vkey.FindElement(By.ClassName("p-item-title")).Text.Replace(":", "");
          variantKeys.Add(keyid, keyname);
          if (db.VariantKeys.Find(keyid) == null) db.VariantKeys.Add(new VariantKey { Id = keyid, Name = keyname });
          foreach (var vval in vkey.FindElement(By.ClassName("p-item-main")).FindElement(By.TagName("ul")).FindElements(By.TagName("li")))
          {
            if (vval.GetAttribute("id") == "")
            {
              var tmpvalstr = vval.FindElement(By.TagName("a")).GetAttribute("title");
              if ((tmpvalstr == "") && vval.FindElement(By.TagName("a")).GetAttribute("innerHTML").Contains("<span"))
                tmpvalstr = vval.FindElement(By.TagName("a")).FindElement(By.TagName("span")).Text;
              if (tmpvalstr != "")
              {
                int varvali = Int32.Parse(vval.FindElement(By.TagName("a")).GetAttribute("data-sku-id"));
                string rrr = vval.FindElement(By.TagName("a")).GetAttribute("innerHTML");
                if (rrr.Contains("img"))
                  tmpvalstr = tmpvalstr + itemsep1 + vval.FindElement(By.TagName("a")).FindElement(By.TagName("img")).GetAttribute("src") + itemsep1 + vval.FindElement(By.TagName("a")).FindElement(By.TagName("img")).GetAttribute("bigpic");
                switch (tmpvi)
                {
                  case 1: variantValues1.Add(varvali, tmpvalstr); break;
                  case 2: variantValues2.Add(varvali, tmpvalstr); break;
                  case 3: variantValues3.Add(varvali, tmpvalstr); break;
                }
                if (db.VariantValues.Find(varvali, keyid) == null) db.VariantValues.Add(new VariantValue { Id = varvali, KeyId = keyid, Name = tmpvalstr.Split(Char.Parse(itemsep1))[0] });
              }
            }
          }
          tmpvi++;
        }
        if (variantValues1.Count > 0)
        {
          String varsource = pageSource.OrtasiniGetir("skuProducts=[", "]");
          if (variantValues2.Count == 0) variantValues2.Add(0, "");
          if (variantValues3.Count == 0) variantValues3.Add(0, "");
          String vtmp = "", vtmp2 = "";
          foreach (var v1 in variantValues1)
          {
            vtmp = v1.Key.ToString();
            foreach (var v2 in variantValues2)
            {
              if (v2.Key != 0)
                vtmp = v1.Key.ToString() + "," + v2.Key.ToString();
              foreach (var v3 in variantValues3)
              {
                if (v3.Key != 0)
                  vtmp = v1.Key.ToString() + "," + v2.Key.ToString() + "," + v3.Key.ToString();
                vtmp2 = "skuPropIds\":\"" + vtmp;
                if (varsource.Contains(vtmp2))
                {
                  vtmp2 = varsource.Substring(varsource.IndexOf(vtmp2) + vtmp2.Length).OrtasiniGetir("{", "}");
                  var tmppv = db.ProductVariants.Find(product.Id, v1.Key, v2.Key, v3.Key);
                  if (tmppv == null)
                  {
                    tmppv = new ProductVariant();
                    db.ProductVariants.Add(tmppv);
                  }
                  tmppv.ProductId = product.Id;
                  if (variantKeys.Keys.Count > 0)
                  {
                    tmppv.Var1Key = variantKeys.Keys.ToList()[0];
                    tmppv.Var1Value = v1.Key;
                    if (db.VariantValues.Find(v1.Key, variantKeys.Keys.ToList()[0]) == null)
                      db.VariantValues.Add(new VariantValue { Id = v1.Key, KeyId = variantKeys.Keys.ToList()[0], Name = v1.Value });
                  }
                  else tmppv.Var1Key = 0;
                  if (variantKeys.Keys.Count > 1)
                  {
                    tmppv.Var2Key = variantKeys.Keys.ToList()[1];
                    tmppv.Var2Value = v2.Key;
                    if (db.VariantValues.Find(v2.Key, variantKeys.Keys.ToList()[1]) == null)
                      db.VariantValues.Add(new VariantValue { Id = v2.Key, KeyId = variantKeys.Keys.ToList()[1], Name = v2.Value });
                  }
                  else tmppv.Var2Key = 0;
                  if (variantKeys.Keys.Count > 2)
                  {
                    tmppv.Var3Key = variantKeys.Keys.ToList()[2];
                    tmppv.Var3Value = v3.Key;
                    if (db.VariantValues.Find(v3.Key, variantKeys.Keys.ToList()[2]) == null)
                      db.VariantValues.Add(new VariantValue { Id = v3.Key, KeyId = variantKeys.Keys.ToList()[2], Name = v3.Value });
                  }
                  else tmppv.Var3Key = 0;
                  tmppv.isActive = vtmp2.OrtasiniGetir("isActivity\":", ",") == "true";
                  tmppv.Price = tmppv.isActive ? decimal.Parse(vtmp2.OrtasiniGetir("actSkuCalPrice\":\"", "\",").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)) : 0;
                  tmppv.OldPrice = decimal.Parse(vtmp2.OrtasiniGetir("skuCalPrice\":\"", "\",").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                  tmppv.Inventory = Int32.Parse(vtmp2.OrtasiniGetir("availQuantity\":", ","));
                  if (v1.Value.Contains(itemsep1))
                  {
                    tmppv.imgThumb = v1.Value.Split(Char.Parse(itemsep1))[1];
                    tmppv.imgBig = v1.Value.Split(Char.Parse(itemsep1))[2];
                  }
                  tmppv.AliSKU = product.Id + "-" + vtmp.Replace(",", "-");
                }
              }
            }
          }
        }
      }
      */
      #endregion
      try
      {
        if (save)
          db.SaveChanges();
      }
      catch (DbEntityValidationException e)
      {
        foreach (var eve in e.EntityValidationErrors)
        {
          MessageBox.Show("Entity of type \"{" + eve.Entry.Entity.GetType().Name + "}\" in state \"{" + eve.Entry.State.ToString() + "}\" has the following validation errors:");
          foreach (var ve in eve.ValidationErrors)
            MessageBox.Show("- Property: \"{" + ve.PropertyName + "}\", Error: \"{" + ve.ErrorMessage + "}\"");
        }
      }

    }

    private void UpdateProduct(ref Product product)
    {
      String pageSource = driver.PageSource;
      String skuProducts = pageSource.OrtasiniGetir("skuProducts=[", "]");
      int rank = 0;
      decimal rating = 0;
      if (pageSource.Contains("class=\"rank-num\""))
      {
        rank = 0; Int32.TryParse(driver.FindElementByXPath("//*[@class=\"rank-num\"]").Text, out rank);
        rating = 0; decimal.TryParse(driver.FindElementByXPath("//*[@class=\"positive-percent\"]").Text.Replace("%", "").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), out rating);
      }
      #region Rate & OrderCount
      decimal Rate = 0;
      int RateCount = 0;
      int OrderCount = 0;
      DateTime? DiscountTime = null;
      if (pageSource.Contains("class=\"percent-num"))
        Rate = decimal.Parse(pageSource.OrtasiniGetir("<span class=\"percent-num\">", "</span>").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
      if (pageSource.Contains("class=\"rantings-num"))
        Int32.TryParse(pageSource.OrtasiniGetir("<span class=\"rantings-num\">", "</span>").Substring(1).Replace(" votes)", "").Replace(" vote)", ""), out RateCount);
      if (pageSource.Contains("class=\"order-num"))
        Int32.TryParse(getText("order-num", "", pageSource).Replace(" orders", "").Replace(" order", ""), out OrderCount);
      if (pageSource.Contains("p-eventtime-left"))
      {
        String etime = pageSource.OrtasiniGetir("p-eventtime-left", "</div>");
        if (etime.Contains("data-role=\"hour\">"))
          DiscountTime = DateTime.Now.AddHours(Int32.Parse(etime.OrtasiniGetir("data-role=\"hour\">", "</span>"))).AddMinutes(Int32.Parse(etime.OrtasiniGetir("data-role=\"minute\">", "</span>"))).AddSeconds(Int32.Parse(etime.OrtasiniGetir("data-role=\"second\">", "</span>")));
        else
          DiscountTime = DateTime.Now.AddDays(Int32.Parse(getText("p-eventtime-left", "", pageSource).Replace("days", "").Replace("left", "").Replace(" ", ""))).Date;

      }
      else
        product.DiscountTime = DateTime.Parse("01.01.1900");
      #endregion
      #region Variants
      Dictionary<int, String> variantKeys = new Dictionary<int, String>(); // 14, Color ; 25, Size vb.
      Dictionary<int, String> variantValues1 = new Dictionary<int, String>(); // 213, Red; 3545, Blue vb.
      Dictionary<int, String> variantValues2 = new Dictionary<int, String>(); // 987, 35; 5746, 36 vb.
      Dictionary<int, String> variantValues3 = new Dictionary<int, String>(); // vb.

      bool dataChanged = false;
      if ((product.Store != null) && ((product.Store.Rank != rank) || (product.Store.Rating != rating)) && (rating != 0) && (rank != 0))
      {
        dataChanged = true;
        product.Store.Rating = rating;
        product.Store.Rank = rank;
      }
      if ((skuProducts.HashMD5() != product.VariantHash) || (Rate != product.Rate) || (RateCount != product.RateCount) || (OrderCount != product.OrderCount))
      {
        product.Rate = Rate;
        product.RateCount = RateCount;
        product.OrderCount = OrderCount;
        dataChanged = true;
        product.VariantHash = skuProducts.HashMD5();
        var tmpvi = 1;
        String infosku = "";
        if (pageSource.Contains("<div id=\"j-product-info-sku\""))
          infosku = pageSource.Substring(pageSource.IndexOf("<div id=\"j-product-info-sku\""));
        while (infosku.Contains("<dl class=\"p-property-item\">"))
        {
          String propertyitem = infosku.OrtasiniGetir("<dl class=\"p-property-item\">", "</dl>");

          int keyid = Int32.Parse(propertyitem.OrtasiniGetir("data-sku-prop-id=\"", "\""));
          String keyname = propertyitem.OrtasiniGetir("p-item-title\">", "</dt>").Replace(":", "");
          variantKeys.Add(keyid, keyname);
          if (db.VariantKeys.Find(keyid) == null) db.VariantKeys.Add(new VariantKey { Id = keyid, Name = keyname });
          while (propertyitem.Contains("</li>"))
          {
            String vval = propertyitem.Substring(0, propertyitem.IndexOf("</li>"));
            var tmpvalstr = vval.OrtasiniGetir("title=\"", "\"");
            if ((tmpvalstr == "") && vval.OrtasiniGetir("<a ", "</a>").Contains("<span"))
              tmpvalstr = vval.OrtasiniGetir("<a ", "</a>").OrtasiniGetir("<span>", "</span>");
            if (tmpvalstr != "")
            {
              int varvali = Int32.Parse(vval.OrtasiniGetir("data-sku-id=\"", "\""));
              if (vval.OrtasiniGetir("<a ", "</a>").Contains("img src="))
                tmpvalstr = tmpvalstr + itemsep1 + vval.OrtasiniGetir("<a ", "</a>").OrtasiniGetir("img src=\"", "\"") + itemsep1 + vval.OrtasiniGetir("<a ", "</a>").OrtasiniGetir("bigpic=\"", "\"");
              switch (tmpvi)
              {
                case 1: variantValues1.Add(varvali, tmpvalstr); break;
                case 2: variantValues2.Add(varvali, tmpvalstr); break;
                case 3: variantValues3.Add(varvali, tmpvalstr); break;
              }
              VariantValue vv = db.VariantValues.Find(varvali, keyid);
              if (vv == null)
              //              if (!listvarvals.Contains(varvali.ToString() + itemsep1 + keyid.ToString()))
              {
                db.VariantValues.Add(new VariantValue { Id = varvali, KeyId = keyid, Name = tmpvalstr.Split(Char.Parse(itemsep1))[0] });
                //listvarvals.Add(varvali.ToString() + itemsep1 + keyid.ToString());
              }
              else vv.Name = tmpvalstr.Split(Char.Parse(itemsep1))[0];
            }
            propertyitem = propertyitem.Substring(propertyitem.IndexOf("</li>") + 2);
          }
          tmpvi++;
          infosku = infosku.Substring(infosku.IndexOf("<dl class=\"p-property-item\">") + 10);
        }
        if (variantValues1.Count > 0)
        {
          if (variantValues2.Count == 0) variantValues2.Add(0, "");
          if (variantValues3.Count == 0) variantValues3.Add(0, "");
          String vtmp = "", vtmp2 = "";
          foreach (var v1 in variantValues1)
          {
            vtmp = v1.Key.ToString();
            foreach (var v2 in variantValues2)
            {
              if (v2.Key != 0)
                vtmp = v1.Key.ToString() + "," + v2.Key.ToString();
              foreach (var v3 in variantValues3)
              {
                if (v3.Key != 0)
                  vtmp = v1.Key.ToString() + "," + v2.Key.ToString() + "," + v3.Key.ToString();
                vtmp2 = "skuPropIds\":\"" + vtmp;
                if (skuProducts.Contains(vtmp2))
                {
                  vtmp2 = skuProducts.Substring(skuProducts.IndexOf(vtmp2) + vtmp2.Length).OrtasiniGetir("{", "}");
                  var tmppv = db.ProductVariants.Find(product.Id, v1.Key, v2.Key, v3.Key);
                  if (tmppv == null)
                  {
                    tmppv = new ProductVariant();
                    db.ProductVariants.Add(tmppv);
                  }
                  tmppv.ProductId = product.Id;
                  if (variantKeys.Keys.Count > 0)
                  {
                    tmppv.Var1Key = variantKeys.Keys.ToList()[0];
                    tmppv.Var1Value = v1.Key;
                    if (db.VariantValues.Find(v1.Key, variantKeys.Keys.ToList()[0]) == null)
                      db.VariantValues.Add(new VariantValue { Id = v1.Key, KeyId = variantKeys.Keys.ToList()[0], Name = v1.Value });
                  }
                  else tmppv.Var1Key = 0;
                  if (variantKeys.Keys.Count > 1)
                  {
                    tmppv.Var2Key = variantKeys.Keys.ToList()[1];
                    tmppv.Var2Value = v2.Key;
                    if (db.VariantValues.Find(v2.Key, variantKeys.Keys.ToList()[1]) == null)
                      db.VariantValues.Add(new VariantValue { Id = v2.Key, KeyId = variantKeys.Keys.ToList()[1], Name = v2.Value });
                  }
                  else tmppv.Var2Key = 0;
                  if (variantKeys.Keys.Count > 2)
                  {
                    tmppv.Var3Key = variantKeys.Keys.ToList()[2];
                    tmppv.Var3Value = v3.Key;
                    if (db.VariantValues.Find(v3.Key, variantKeys.Keys.ToList()[2]) == null)
                      db.VariantValues.Add(new VariantValue { Id = v3.Key, KeyId = variantKeys.Keys.ToList()[2], Name = v3.Value });
                  }
                  else tmppv.Var3Key = 0;
                  tmppv.isActive = vtmp2.OrtasiniGetir("isActivity\":", ",") == "true";
                  tmppv.Price = decimal.Parse(vtmp2.OrtasiniGetir("actSkuCalPrice\":\"", "\",").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                  tmppv.OldPrice = decimal.Parse(vtmp2.OrtasiniGetir("skuCalPrice\":\"", "\",").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                  int inv = 0;
                  Int32.TryParse(vtmp2.OrtasiniGetir("availQuantity\":", ","), out inv);
                  tmppv.Inventory = inv;
                  if (v1.Value.Contains(itemsep1))
                  {
                    tmppv.imgThumb = v1.Value.Split(Char.Parse(itemsep1))[1];
                    tmppv.imgBig = v1.Value.Split(Char.Parse(itemsep1))[2];
                  }
                  tmppv.AliSKU = product.Id + "-" + vtmp.Replace(",", "-");
                }
              }
            }
          }
        }

      }
      #endregion
      if (dataChanged)
        product.UpdateDT = DateTime.Now;
      product.LastCheckDT = DateTime.Now;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //textBox1.Clear();
      string[] args = Environment.GetCommandLineArgs();
      //foreach (var item in db.VariantValues)
      //{
      //  if (!listvarvals.Contains(item.Id + itemsep1 + item.KeyId))
      //  {
      //    listvarvals.Add(item.Id.ToString() + itemsep1 + item.KeyId.ToString());
      //  }
      //}
      if (args.Count() > 1)
      {
        tb_mod.Text = args[1];
        tb_modcount.Text = args[2];
      }
      if (args.Count() > 2)
        tb_vpncount.Text = args[3];
      ///if (tb_mod.Text != "")
      //  button1_Click(null, null);
      //for (int i = 1; i < args.Count(); i++)
      //{
      //  if (args[i] == "mod") tb_mod.Text = args[i + 1];
      //  if (args[i] == "modcount") tb_modcount.Text = args[i + 1];
      //}
      //button1_Click(button1, null);
    }


    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (_driver != null)
        driver.Quit();
      if (service != null)
        service.Dispose();
    }

    private async void button2_Click(object sender, EventArgs e)
    {
      //if (_driver != null)
      //  reCreateDriver();
      //driver.Navigate().GoToUrl("about:blank"); // ("http://www.whoishostingthis.com/tools/user-agent/");
      //return;
      DateTime lastUpdate;
      IQueryable<Product> prods = null;
      //if (db.VariantValues.Find(varvali, keyid) == null)

      List<String> productList = new List<String>();
      while (true)
      {
        lastUpdate = DateTime.Now;
        int cnt = 1;
        while ((prods == null) || (prods.Count() > 0))
        {
          DateTime dt0 = DateTime.ParseExact("01.01.2000", "dd.MM.yyyy", CultureInfo.InvariantCulture);
          Int32 m = 0;
          Int32 mc = 1;
          Int32.TryParse(tb_mod.Text, out m);
          Int32.TryParse(tb_modcount.Text, out mc);
          if ((tb_mod.Text != "") && (tb_modcount.Text != ""))
            prods = db.Products.Where(x => (x.DiscountTime < DateTime.Now) && (x.DiscountTime > dt0) && (x.IdentId % mc == m - 1)); // db.Products.Where(x => x.Id == "1204554744");//
          else
            prods = db.Products.Where(x => (x.DiscountTime < DateTime.Now) && (x.DiscountTime > dt0));
          foreach (var prod in prods) // .Take(1)
          {
            productList.Add(prod.Id);
          }
          cnt = 1;
          foreach (var prod in productList) // .Take(1)
          {
            this.Text = cnt.ToString() + "/" + productList.Count().ToString("###,###,###");
            Product pr = db.Products.Find(prod);
            await Task.Factory.StartNew(() => Navigate(pr.Link));
            await Task.Factory.StartNew(() => UpdateProduct(ref pr));
            driver.Navigate().Back();
            reCreateDriver();
            cnt++;
            db.SaveChanges();
          }
        }
        prods = db.Products.Where(x => x.UpdateDT < lastUpdate);
        productList.Clear();
        foreach (var prod in prods) // .Take(1)
        {
          productList.Add(prod.Id);
        }
        cnt = 1;
        foreach (var prod in productList)
        {
          this.Text = cnt.ToString() + "/" + productList.Count().ToString("###,###,###");
          Product pr = db.Products.Find(prod);
          await Task.Factory.StartNew(() => Navigate(pr.Link));
          //Navigate(pr.Link);
          await Task.Factory.StartNew(() => UpdateProduct(ref pr));
          driver.Navigate().Back();
          reCreateDriver();
          cnt++;
          db.SaveChanges();
        };
      }
    }

    private void Navigate(string link)
    {
      try
      {
        String linkprefix = "";
        int fazlalik = (Int32.Parse(tb_mod.Text) + (Int32.Parse(tb_modcount.Text) * (activeVpnNo % Int32.Parse(tb_vpncount.Text)))) - 15;
        if (fazlalik < 20)//şimdilik buraya girmesin...
          if (fazlalik > 0)
          {
            switch (fazlalik)
            {
              case 1:
                linkprefix = "http://ali01.gear.host/index.aspx?link=";
                break;
              case 2:
                linkprefix = "http://erceskar.rf.gd/index.php?link=";
                break;
              case 3:
                linkprefix = "http://ali02.somee.com/index.aspx?link=";
                break;
            }
          }
        if (!link.StartsWith(linkprefix))
          link = linkprefix + link;
        driver.Navigate().GoToUrl(link);
        int stime = 1;
        while (driver.Url.Replace("http://", "https://") != link.Replace("http://", "https://"))
        {
          driver.Navigate().GoToUrl("bostu.html");
          if (stime % 3 == 0)
            reCreateDriver();
          else
          {
            Thread.Sleep(stime * 1000);
            driver.Navigate().Back();
          }
          driver.Navigate().GoToUrl(link);
          stime++;
        }
      }
      catch
      {
        reCreateDriver();
        Navigate(link);
      }
    }

    private void reCreateDriver()
    {
      driver.Quit();
      //driver.Close();
      driver.Dispose();
      _driver = null;
      service.Dispose();
      service = null;
    }

    private async void button3_Click(object sender, EventArgs e)
    {
      var settings = new TorSharpSettings
      {
        ZippedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorZipped"),
        ExtractedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorExtracted"),
        PrivoxyPort = 1337,
        TorSocksPort = 1338,
        TorControlPort = 1339,
        TorControlPassword = "foobar"
      };

      // download tools
      await new TorSharpToolFetcher(settings, new HttpClient()).FetchAsync();

      // execute
      var proxy = new TorSharpProxy(settings);
      var handler = new HttpClientHandler
      {
        Proxy = new WebProxy(new Uri("http://localhost:" + settings.PrivoxyPort))
      };
      var httpClient = new HttpClient(handler);
      await proxy.ConfigureAndStartAsync();
      Console.WriteLine(await httpClient.GetStringAsync("http://icanhazip.com"));
      await proxy.GetNewIdentityAsync();
      Console.WriteLine(await httpClient.GetStringAsync("http://icanhazip.com"));
      proxy.Stop();
      return;
      Process p = new Process();
      p.StartInfo = new ProcessStartInfo(@"C:\Users\Erci\Desktop\Tor Browser\Browser\TorBrowser\Tor\tor.exe", "ControlPort 9051 CircuitBuildTimeout 10");

      //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      p.Start();
      Thread.Sleep(5000);
      Regex regex = new Regex("\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}", RegexOptions.Multiline);

      {

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ipadresimnedir.com/");

        request.Proxy = new WebProxy("127.0.0.1:8118");
        request.KeepAlive = false;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {

          using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
          {

            //webBrowser1.DocumentText = reader.ReadToEnd();

            //Regex regex = new Regex("value=\"([0-9]*)\\.([0-9]*)\\.([0-9]*)\\.([0-9]*)\"");

            string contenu = reader.ReadToEnd();
            Console.WriteLine(regex.Match(contenu).Groups[0].Value);

          }

        }

        Console.Write("en attente : continuez ?");
        string line = Console.ReadLine();

        //if (line != "y") break;
      }
      return;
      // skuProducts
      //      reCreateDriver();
      driver.Navigate().GoToUrl("about:blank");
      return;
      for (int i = 0; i < textBox2.Lines.Length; i++)
      {
        //WebBrowser wb = new WebBrowser();
        //wb.Navigate(textBox2.Lines[i]);
        //wb.
        //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(textBox2.Lines[i]);
        //HttpWebResponse response = (HttpWebResponse)req.GetResponse();
        //Stream streamResponse = response.GetResponseStream();
        //StreamReader streamRead = new StreamReader(streamResponse);
        //String str = streamRead.ReadToEnd();

        //WebClient wc = new WebClient();
        /*
        
        wc.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch, br");
        wc.Headers.Add(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.8,en-US;q=0.6,en;q=0.4");
        //wc.Headers.Add(HttpRequestHeader.Connection, "keep-alive");
        wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
        wc.Headers.Add(HttpRequestHeader.Referer, "https://www.aliexpress.com/category/200003482/dresses.html?d=n&isViewCP=y&CatId=200003482&catName=dresses&spm=2114.11010108.101.3.IMlQTN&g=y&blanktest=0&tc=af");
        String str = wc.DownloadString("http://www.whatsmyua.info/");
        */
        //wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        //driver.Navigate().GoToUrl(textBox2.Lines[i]);
        //Console.WriteLine(i.ToString() + "/" + textBox2.Lines.Length.ToString() + DateTime.Now.ToLongTimeString());
        //if (!str.Contains("skuProducts")) //if (!driver.Url.StartsWith(textBox2.Lines[i]))
        //{
        //  int j = 0;
        //  while ((j < 5) && (!str.Contains("skuProducts"))) //(!driver.Url.StartsWith(textBox2.Lines[i])))
        //  {
        //    Console.WriteLine("Bulamadı : " + j.ToString());
        //    str = wc.DownloadString(textBox2.Lines[i]);
        //    Thread.Sleep(1000);
        //    j++;
        //  }
        //  if (!str.Contains("skuProducts")) //(!driver.Url.StartsWith(textBox2.Lines[i]))
        //    i = textBox2.Lines.Length + 10;
        //}

      }

    }

    private async void button4_Click(object sender, EventArgs e)
    {
      //var settings = new TorSharpSettings
      //{
      //  ZippedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorZipped"),
      //  ExtractedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorExtracted"),
      //  PrivoxyPort = 1337,
      //  TorSocksPort = 1338,
      //  TorControlPort = 1339,
      //  TorControlPassword = "erci1234"
      //  //,ToolRunnerType = ToolRunnerType.Simple
      //};

      //await new TorSharpToolFetcher(settings, new HttpClient()).FetchAsync();
      //https://translate.google.com/translate?hl=tr&sl=en&tl=tr&u=
      int torport = 9151;
      string torpass = "erci1234";
      IPEndPoint torendPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), torport);
      torserver = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      torserver.Connect(torendPoint);
      torserver.Send(Encoding.ASCII.GetBytes("AUTHENTICATE \"" + torpass + "\"" + Environment.NewLine));
      //byte[] data = new byte[1024];
      //int receivedDataLength = torserver.Receive(data);
      //string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
      //Request a new Identity
      //      torserver.Shutdown(SocketShutdown.Both);
      //      torserver.Close();

      //var proxy = new TorSharpProxy(settings);
      //var sw = new SocksWebProxy(new ProxyConfig(
      //  //This is an internal http->socks proxy that runs in process
      //  IPAddress.Parse("127.0.0.1"),
      //  //This is the port your in process http->socks proxy will run on
      //  9151,
      //  //This could be an address to a local socks proxy (ex: Tor / Tor Browser, If Tor is running it will be on 127.0.0.1)
      //  IPAddress.Parse("127.0.0.1"),
      //  //This is the port that the socks proxy lives on (ex: Tor / Tor Browser, Tor is 9150)
      //  1338,
      //  //This Can be Socks4 or Socks5
      //  ProxyConfig.SocksVersion.Five
      //  ));
      //var handler = new HttpClientHandler
      //{
      //  Proxy = sw//new WebProxy(new Uri("http://localhost:12345"))// + settings.PrivoxyPort))
      //};

      var socksProxy = new Socks5ProxyClient("localhost", 1338);
      var handler = new ProxyHandler(socksProxy);

      //var wc = new WebClient(); // 3 sayfada bir patlıyor (ip değiştirmek gerekiyor)
      //WebProxy wp = new WebProxy("http://localhost:" + settings.PrivoxyPort);
      //wc.Proxy = wp;

      var httpClient = new HttpClient(handler);
      //httpClient.DefaultRequestHeaders.Add("User-Agent",
      //                     "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident / 6.0)");

      //httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
      //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

      //await proxy.ConfigureAndStartAsync();

      // tor.exe -f .\torrc-defaults
      String str = "";
      int i = 1;
      int waitasecond = 0;
      DateTime bastar = DateTime.Now;
      foreach (var item in textBox2.Lines)
      {
        i++;
        Console.WriteLine(i.ToString() + " " + (DateTime.Now.Subtract(bastar).TotalMilliseconds / i).ToString("###,###.##") + " milisaniyede 1 sayfa");
        //str = wc.DownloadString(item);//
        str = await httpClient.GetStringAsync(item);
        //Task t = Task.Factory.StartNew(() =>
        //{
        //  DownloadPageAsync(item, handler);
        //});
        waitasecond = 0;
        while (!str.Contains("skuProducts"))
        {
          str = await httpClient.GetStringAsync("http://icanhazip.com") + " > ";
          //await proxy.GetNewIdentityAsync();
          //proxy.Stop();
          //await proxy.ConfigureAndStartAsync();
          ChangeTorIp();
          Thread.Sleep(waitasecond * 1000);
          Console.WriteLine((waitasecond * 1000).ToString() + "sn beklendi");
          waitasecond++;
          str += await httpClient.GetStringAsync("http://icanhazip.com");
          Console.WriteLine(str);
          str = await httpClient.GetStringAsync(item);
        }
      }
      //Console.WriteLine();
      //Console.WriteLine(await httpClient.GetStringAsync("http://icanhazip.com"));
      //proxy.Stop();

    }

    static async void DownloadPageAsync(string url, ProxyHandler handler)
    {
      // ... Target page.
      string page = url; //"http://en.wikipedia.org/";

      // ... Use HttpClient.
      using (HttpClient client = new HttpClient(handler))
      using (HttpResponseMessage response = await client.GetAsync(page))
      using (HttpContent content = response.Content)
      {
        // ... Read the string.
        string result = await content.ReadAsStringAsync();

        // ... Display the result.
        if (result != null &&
            result.Length >= 50)
        {
          Console.WriteLine(result.Substring(0, 50) + "...");
        }
      }
    }

    private bool ChangeTorIp()
    {
      torserver.Send(Encoding.ASCII.GetBytes("SIGNAL NEWNYM" + Environment.NewLine));
      byte[] data = new byte[1024];
      int receivedDataLength = torserver.Receive(data);
      string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
      return stringData.Contains("250");
    }

    private async void button5_Click(object sender, EventArgs e)
    {
    }

    private void button6_Click(object sender, EventArgs e)
    {
      Dictionary<String, String> list = new Dictionary<string, string>();
      int pagenum = 1;
      WebClient client = new WebClient();
      while ((textBox1.Lines.Count() > 0) && (textBox1.Lines[0] != ""))
        try
        {
          String pageSource = client.DownloadString(textBox1.Lines[0] + "&page=" + pagenum.ToString());
          //Navigate(textBox1.Lines[0] + "&page=" + pagenum.ToString());
          while (!((driver.FindElementByClassName("search-count").Text == "0") || (driver.PageSource.Contains("did not match any products"))))
          {
            DateTime ft = DateTime.Now.AddSeconds(1);
            int y = 100;
            //for (int i = 0; i < 200; i++)
            var tmppgsrc = driver.PageSource;
            while (DateTime.Now < ft) //(tmppgsrc == driver.PageSource) // 
            {
              driver.ExecuteScript("window.scrollTo(0, " + y.ToString() + ")"); // (document.body.scrollHeight)
              y = y + 100;
            }
            y = 0;
            foreach (var item in driver.FindElementById("list-items").FindElement(By.TagName("ul")).FindElements(By.TagName("li")))
            {
              y++;
              bool kriterlereuygun = false;
              String satirsource = item.GetAttribute("innerHTML"); //Feedback
              int storerank = 0;
              int storerankkri = 0;
              decimal storerating = 0;
              decimal storeratingkri = 0;
              decimal starrating = 0;
              decimal ratekri = 4.5M;
              int feedback = 0;
              int feedbackkri = 20;
              int orderkri = 100;
              int order = 0;
              if (satirsource.Contains("title=\"Star Rating: "))
                decimal.TryParse(satirsource.OrtasiniGetir("title=\"Star Rating: ", " ").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), out starrating);
              if (satirsource.Contains("title=\"Feedback"))
                Int32.TryParse(satirsource.OrtasiniGetir("title=\"Feedback(", ")"), out feedback);
              if (satirsource.Contains("<em title=\"Total Orders\">"))
                Int32.TryParse(satirsource.OrtasiniGetir("<em title=\"Total Orders\">", "</em>").Replace("Orders", "").Replace(" ", "").Replace("(", "").Replace(")", ""), out order);
              if (satirsource.Contains("class=\"score-icon"))
                Int32.TryParse(satirsource.OrtasiniGetir("class=\"score-icon", ">").OrtasiniGetir("feedbackscore=\"", "\"").Replace(",", ""), out storerank);
              if (satirsource.Contains("class=\"score-icon"))
                decimal.TryParse(satirsource.OrtasiniGetir("class=\"score-icon", ">").OrtasiniGetir("sellerpositivefeedbackpercentage=\"", "\"").Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), out storerating);
              kriterlereuygun = (
                (starrating >= ratekri)
                && (feedback >= feedbackkri)
                && (order >= orderkri)
                && (storerank >= storerankkri)
                && (storerating >= storeratingkri)
              );
              if ((kriterlereuygun) && (satirsource.Contains("class=\"info")))
              {
                var ele = item.FindElement(By.TagName("a"));
                var link = "http://" + ele.GetAttribute("href").OrtasiniGetir("www.", ".html", true); // başında http olmazsa driver linki açmıyor...
                var img = "";
                try
                {
                  img = ele.FindElement(By.TagName("img")).GetAttribute("src");
                  if (img.EndsWith("_220x220.jpg"))
                    img = img.Substring(0, img.Length - 12);
                }
                catch (Exception) { img = ""; };
                String pricetext = item.FindElement(By.ClassName("item")).FindElement(By.ClassName("info")).GetAttribute("innerHTML");
                pricetext = pricetext.OrtasiniGetir("<span class=\"price price-m\">", "</div>");
                pricetext = pricetext.OrtasiniGetir("<span class=\"value\" itemprop=\"price\">", "</span>");
                img = img + itemsep1 + pricetext + itemsep1 + starrating + itemsep1 + feedback + itemsep1 + order + itemsep1 + storerank + itemsep1 + storerating;
                if (!list.Keys.ToList().Exists(xx => xx == link))
                  list.Add(link, img);
              }
              this.Text = y.ToString();
            }
            for (int i = 0; i < list.Count(); i++)
            {
              String strx = list.Keys.ToList()[i].Split('/')[5];
              var prod = db.Products.Find(strx.Substring(0, strx.IndexOf(".")));
              bool gir = true;
              bool changed = false;
              if (prod != null)
              {
                //                changed = changed || (prod.Rate - decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[2]) != 0);
                prod.Rate = decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[2]);
                //                changed = changed || (prod.Last6MOrderCount - Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[3]) != 0);
                prod.Last6MOrderCount = Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[3]);
                //                changed = changed || (prod.OrderCount - Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[4]) != 0);
                prod.OrderCount = Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[4]);
                //                changed = changed || (prod.Store.Rank - Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[5]) != 0);
                prod.Store.Rank = Int32.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[5]);
                //                changed = changed || (prod.Store.Rating - decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[6]) != 0);
                prod.Store.Rating = decimal.Parse(list.Values.ToList()[i].Split(Char.Parse(itemsep1))[6]);
                if (prod.PriceRange == null)
                  prod.PriceRange = list.Values.ToList()[i].Split(Char.Parse(itemsep1))[1];
                gir = (prod.PriceRange != list.Values.ToList()[i].Split(Char.Parse(itemsep1))[1]) && (list.Values.ToList()[i].Split(Char.Parse(itemsep1))[1].Contains("-")); // Fiyat değişmiş...
                changed = true;
                prod.LastCheckDT = DateTime.Now;
              }
              if (gir)
              {
                //await Task.Factory.StartNew(() => 
                getLink(list.Keys.ToList()[i], list.Values.ToList()[i]);
                  //);
                driver.Navigate().Back();
                reCreateDriver();
                changed = false;
              }
              if (changed)
                db.SaveChanges();
              this.Text = LastStore + " " + pagenum.ToString() + "(" + i.ToString() + "/" + list.Count() + ")";
            }
            this.Text = LastStore + " " + pagenum + "(" + list.Count() + "/" + list.Count() + ")";
            pagenum++;
            //driver.Quit();
            //driver.Dispose();
            //_driver = null;
            list.Clear();
            driver.Navigate().Back();
            reCreateDriver();
            Navigate(textBox1.Lines[0] + "&page=" + pagenum.ToString());
            //while (driver.Url != textBox1.Lines[0] + "&page=" + pagenum.ToString())
            //{
            //  Thread.Sleep(stime * 1000);
            //  driver.Navigate().Back();
            //  driver.Navigate().GoToUrl(textBox1.Lines[0] + "&page=" + pagenum.ToString());
            //  stime++;
            //}
            //stime = 1;
          }
          textBox1.Lines = textBox1.Lines.Skip(tb_modcount.Text == "" ? 1 : Int32.Parse(tb_modcount.Text)).ToArray();
          pagenum = 1;
        }
        catch { }
      driver.Close();
      service.Dispose();

    }
  }
  class TSList<T>
  {
    private List<T> _list = new List<T>();
    private object _sync = new object();
    public void Add(T value)
    {
      lock (_sync)
      {
        _list.Add(value);
      }
    }
    public T Find(Predicate<T> predicate)
    {
      lock (_sync)
      {
        return _list.Find(predicate);
      }
    }
    public T FirstOrDefault()
    {
      lock (_sync)
      {
        return _list.FirstOrDefault();
      }
    }
  }

}

