using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

public class Handler1 : IHttpHandler
{
    // 文件路径
    private string dataFilePath;
    private string deliveryFilePath;
    private string cartFilePath;

    public Handler1()
    {
        // 获取文件路径
        dataFilePath = HttpContext.Current.Server.MapPath("products.txt");
        deliveryFilePath = HttpContext.Current.Server.MapPath("delivery_records.txt");
        cartFilePath = HttpContext.Current.Server.MapPath("cart_data.txt");

        // 确保文件存在
        EnsureFilesExist();
    }

    private void EnsureFilesExist()
    {
        try
        {
            // 如果产品文件不存在，创建丰富的农产品数据
            if (!File.Exists(dataFilePath))
            {
                CreateSampleData();
            }

            // 如果配送记录文件不存在，创建空文件
            if (!File.Exists(deliveryFilePath))
            {
                File.WriteAllText(deliveryFilePath, "delivery_time,product_id,product_name,origin_name,destination,province,transport_mode\n", Encoding.UTF8);
            }

            // 如果购物车文件不存在，创建空文件
            if (!File.Exists(cartFilePath))
            {
                File.WriteAllText(cartFilePath, "user_id,product_id,quantity,added_time\n", Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            HttpContext.Current.Response.Write("{\"error\":\"初始化文件失败: " + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void CreateSampleData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("product_id,product_name,category,price,origin_lat,origin_lng,origin_name,origin_type,description,stock,province");

        // 创建200个产品数据，每个产品在多个省份
        int productId = 1;
        Random rand = new Random();

        // 产品种类定义
        var products = new[]
        {
            new { Name = "新鲜胡萝卜", Category = "蔬菜", BasePrice = 3.0m, Description = "新鲜采摘的胡萝卜，富含维生素A" },
            new { Name = "有机西红柿", Category = "蔬菜", BasePrice = 5.5m, Description = "有机种植，无农药残留" },
            new { Name = "本地大白菜", Category = "蔬菜", BasePrice = 2.5m, Description = "本地种植，口感鲜美" },
            new { Name = "绿色黄瓜", Category = "蔬菜", BasePrice = 4.0m, Description = "当天采摘，新鲜直达" },
            new { Name = "紫皮茄子", Category = "蔬菜", BasePrice = 6.0m, Description = "有机种植，营养丰富" },
            new { Name = "甜玉米", Category = "蔬菜", BasePrice = 8.0m, Description = "非转基因，自然成熟" },
            new { Name = "土豆", Category = "蔬菜", BasePrice = 3.0m, Description = "本地沙地土豆，口感绵软" },
            new { Name = "青椒", Category = "蔬菜", BasePrice = 4.5m, Description = "新鲜青椒，微辣爽口" },
            new { Name = "红富士苹果", Category = "水果", BasePrice = 6.5m, Description = "脆甜多汁，产地直供" },
            new { Name = "香蕉", Category = "水果", BasePrice = 3.8m, Description = "热带香蕉，自然成熟" },
            new { Name = "橙子", Category = "水果", BasePrice = 5.5m, Description = "赣南脐橙，甜度高" },
            new { Name = "巨峰葡萄", Category = "水果", BasePrice = 12.0m, Description = "新鲜葡萄，皮薄多汁" },
            new { Name = "菠菜", Category = "蔬菜", BasePrice = 4.0m, Description = "新鲜菠菜，富含铁质" },
            new { Name = "芹菜", Category = "蔬菜", BasePrice = 3.2m, Description = "本地芹菜，爽脆可口" },
            new { Name = "大蒜", Category = "蔬菜", BasePrice = 8.0m, Description = "紫皮大蒜，香味浓郁" },
            new { Name = "生姜", Category = "蔬菜", BasePrice = 12.0m, Description = "老姜，辣味足" },
            new { Name = "红薯", Category = "蔬菜", BasePrice = 2.8m, Description = "沙地红薯，香甜软糯" },
            new { Name = "洋葱", Category = "蔬菜", BasePrice = 3.5m, Description = "紫皮洋葱，营养丰富" },
            new { Name = "莲藕", Category = "蔬菜", BasePrice = 6.5m, Description = "新鲜莲藕，清脆可口" },
            new { Name = "山药", Category = "蔬菜", BasePrice = 9.0m, Description = "铁棍山药，药用价值高" }
        };

        // 省份坐标数据
        var provinces = new Dictionary<string, (double lat, double lng, double priceFactor)>
        {
            { "北京", (39.9042, 116.4074, 1.1) },
            { "上海", (31.2304, 121.4737, 1.2) },
            { "广东", (23.1291, 113.2644, 1.1) },
            { "江苏", (32.0607, 118.7969, 1.0) },
            { "浙江", (30.278, 120.1528, 1.0) },
            { "山东", (36.6512, 117.1201, 0.9) },
            { "四川", (30.5728, 104.0668, 0.95) },
            { "陕西", (34.3416, 108.9398, 0.9) },
            { "湖北", (30.5928, 114.3055, 0.95) },
            { "湖南", (28.2282, 112.9388, 0.95) },
            { "河南", (34.7529, 113.6653, 0.9) },
            { "河北", (38.0489, 114.5121, 0.9) },
            { "辽宁", (41.8057, 123.4315, 0.95) },
            { "吉林", (43.8171, 125.323, 0.95) },
            { "黑龙江", (45.8038, 126.534, 0.9) },
            { "山西", (37.8706, 112.5489, 0.9) },
            { "安徽", (31.8257, 117.2264, 0.95) },
            { "福建", (24.4798, 118.0894, 1.0) },
            { "江西", (28.6831, 115.8581, 0.95) },
            { "云南", (25.0751, 102.7413, 1.0) },
            { "贵州", (26.8515, 104.8472, 0.95) },
            { "甘肃", (36.0611, 103.8343, 0.85) },
            { "新疆", (43.8171, 87.5804, 1.0) },
            { "广西", (22.817, 108.3665, 0.95) },
            { "海南", (20.028, 110.3203, 1.1) },
            { "重庆", (29.5651, 106.5516, 0.95) },
            { "天津", (39.3434, 117.3616, 1.05) },
            { "宁夏", (38.4862, 106.2325, 0.9) },
            { "青海", (36.6232, 101.7787, 0.85) },
            { "西藏", (29.6469, 91.1175, 1.1) }
        };

        // 为每个产品在多个省份创建产地
        foreach (var product in products)
        {
            // 为每个产品随机选择5-8个省份
            var selectedProvinces = provinces.Keys
                .OrderBy(x => rand.Next())
                .Take(rand.Next(5, 9))
                .ToList();

            foreach (var province in selectedProvinces)
            {
                var provinceData = provinces[province];

                // 为每个省份创建2-3个产地
                int locationCount = rand.Next(2, 4);
                for (int i = 1; i <= locationCount; i++)
                {
                    // 随机偏移坐标（在省份中心附近）
                    double latOffset = (rand.NextDouble() - 0.5) * 0.8;
                    double lngOffset = (rand.NextDouble() - 0.5) * 1.2;

                    double lat = Math.Round(provinceData.lat + latOffset, 4);
                    double lng = Math.Round(provinceData.lng + lngOffset, 4);

                    // 计算价格（基于基础价格和省份系数）
                    decimal price = product.BasePrice * (decimal)provinceData.priceFactor;
                    price = Math.Round(price + (decimal)(rand.NextDouble() * 0.6 - 0.3), 1);

                    // 随机库存
                    int stock = rand.Next(100, 1001);

                    // 产地类型
                    string originType = rand.Next(0, 2) == 0 ? "果蔬基地" : "产地直销店";

                    // 产地名称
                    string originName = $"{province}{product.Name.Substring(0, 2)}{originType}{i}号";

                    // 添加到CSV
                    sb.AppendLine($"{productId},{product.Name},{product.Category},{price},{lat},{lng},{originName},{originType},{product.Description},{stock},{province}");

                    productId++;

                    if (productId > 200) break;
                }
                if (productId > 200) break;
            }
            if (productId > 200) break;
        }

        File.WriteAllText(dataFilePath, sb.ToString(), Encoding.UTF8);
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.Charset = "utf-8";

        // 允许跨域
        context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
        context.Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");

        try
        {
            string action = context.Request["action"] ?? "getAllProducts";

            switch (action.ToLower())
            {
                case "getallproducts":
                    GetAllProducts(context);
                    break;
                case "getproductsbypage":
                    GetProductsByPage(context);
                    break;
                case "getproductbyid":
                    GetProductById(context);
                    break;
                case "searchproducts":
                    SearchProducts(context);
                    break;
                case "filterbycategory":
                    FilterByCategory(context);
                    break;
                case "filterbyprovince":
                    FilterByProvince(context);
                    break;
                case "getproductsbyname":
                    GetProductsByName(context);
                    break;
                case "deliverproduct":
                    DeliverProduct(context);
                    break;
                case "adddeliveryrecord":
                    AddDeliveryRecord(context);
                    break;
                case "getsearchsuggestions":
                    GetSearchSuggestions(context);
                    break;
                case "test":
                    TestConnection(context);
                    break;
                case "getheatmapdata":
                    GetHeatmapData(context);
                    break;
                case "getprovincedata":
                    GetProvinceData(context);
                    break;
                case "getprovincestats":
                    GetProvinceStats(context);
                    break;
                case "getproductsbyprovince":
                    GetProductsByProvince(context);
                    break;
                case "getdeliveryrecords":
                    GetDeliveryRecords(context);
                    break;
                case "getcartdata":
                    GetCartData(context);
                    break;
                case "addtocart":
                    AddToCart(context);
                    break;
                case "updatecart":
                    UpdateCart(context);
                    break;
                case "clearcart":
                    ClearCart(context);
                    break;
                default:
                    GetAllProducts(context);
                    break;
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" +
                CleanForJson(ex.Message) + "\",\"stackTrace\":\"" +
                CleanForJson(ex.StackTrace) + "\"}");
        }
    }

    private void GetAllProducts(HttpContext context)
    {
        try
        {
            List<Product> products = LoadProducts();
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(products);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetProductsByPage(HttpContext context)
    {
        try
        {
            int page = Convert.ToInt32(context.Request["page"] ?? "1");
            int pageSize = Convert.ToInt32(context.Request["pageSize"] ?? "12");

            List<Product> products = LoadProducts();

            // 计算总页数
            int totalCount = products.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // 获取当前页数据
            var pageProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                success = true,
                products = pageProducts,
                currentPage = page,
                totalPages = totalPages,
                totalCount = totalCount
            };

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(result);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetProductById(HttpContext context)
    {
        try
        {
            int productId = Convert.ToInt32(context.Request["productId"]);
            List<Product> products = LoadProducts();
            Product product = products.FirstOrDefault(p => p.product_id == productId);

            if (product != null)
            {
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(product);
                context.Response.Write(json);
            }
            else
            {
                context.Response.Write("{\"error\":\"产品不存在\"}");
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void SearchProducts(HttpContext context)
    {
        try
        {
            string keyword = context.Request["keyword"] ?? "";
            List<Product> products = LoadProducts();

            var filteredProducts = products.Where(p =>
                p.product_name.Contains(keyword) ||
                p.origin_name.Contains(keyword) ||
                p.description.Contains(keyword) ||
                p.category.Contains(keyword) ||
                (p.province != null && p.province.Contains(keyword))
            ).ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(filteredProducts);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetProductsByName(HttpContext context)
    {
        try
        {
            string productName = context.Request["productName"] ?? "";
            List<Product> products = LoadProducts();

            var filteredProducts = products.Where(p => p.product_name == productName).ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(filteredProducts);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void FilterByCategory(HttpContext context)
    {
        try
        {
            string category = context.Request["category"] ?? "";
            List<Product> products = LoadProducts();

            var filteredProducts = category.ToLower() == "all"
                ? products
                : products.Where(p => p.category == category).ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(filteredProducts);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void FilterByProvince(HttpContext context)
    {
        try
        {
            string province = context.Request["province"] ?? "";
            List<Product> products = LoadProducts();

            var filteredProducts = string.IsNullOrEmpty(province)
                ? products
                : products.Where(p => p.province == province).ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(filteredProducts);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetProductsByProvince(HttpContext context)
    {
        try
        {
            string province = context.Request["province"] ?? "all";
            List<Product> products = LoadProducts();

            var filteredProducts = province == "all"
                ? products
                : products.Where(p => p.province == province).ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                province = province,
                count = filteredProducts.Count,
                products = filteredProducts
            });
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void DeliverProduct(HttpContext context)
    {
        try
        {
            int productId = Convert.ToInt32(context.Request["productId"]);
            string productName = context.Request["productName"] ?? "";
            string province = context.Request["province"] ?? "";

            List<Product> products = LoadProducts();
            Product product = products.FirstOrDefault(p => p.product_id == productId);

            if (product != null)
            {
                // 减少库存
                int stockReduction = 1;
                product.stock = Math.Max(0, product.stock - stockReduction);
                SaveProducts(products);

                // 记录配送
                RecordDelivery(productId, productName, product.origin_name, "江苏海洋大学", product.province, "truck");

                context.Response.Write("{\"success\":true,\"message\":\"全国配送成功\",\"originName\":\"" +
                    product.origin_name + "\",\"province\":\"" + product.province + "\",\"stock\":\"" + product.stock + "\"}");
            }
            else
            {
                context.Response.Write("{\"success\":false,\"error\":\"产品不存在\"}");
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void AddDeliveryRecord(HttpContext context)
    {
        try
        {
            int productId = Convert.ToInt32(context.Request["productId"]);
            string productName = context.Request["productName"] ?? "";
            string originName = context.Request["originName"] ?? "";
            string destination = context.Request["destination"] ?? "江苏海洋大学";
            string province = context.Request["province"] ?? "";
            string transportMode = context.Request["transportMode"] ?? "truck";

            RecordDelivery(productId, productName, originName, destination, province, transportMode);

            context.Response.Write("{\"success\":true,\"message\":\"全国配送记录已保存\"}");
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetSearchSuggestions(HttpContext context)
    {
        try
        {
            string keyword = context.Request["keyword"] ?? "";
            List<Product> products = LoadProducts();

            var suggestions = new List<object>();

            // 产品名称建议
            var productNames = products
                .Where(p => p.product_name.Contains(keyword))
                .Select(p => p.product_name)
                .Distinct()
                .Take(8)
                .ToList();

            foreach (var name in productNames)
            {
                suggestions.Add(new
                {
                    text = name,
                    type = "产品",
                    icon = GetProductIcon(name, products.First(p => p.product_name == name).category),
                    searchType = "product"
                });
            }

            // 产地建议
            var originNames = products
                .Where(p => p.origin_name.Contains(keyword))
                .Select(p => p.origin_name)
                .Distinct()
                .Take(5)
                .ToList();

            foreach (var name in originNames)
            {
                suggestions.Add(new
                {
                    text = name,
                    type = "产地",
                    icon = "📍",
                    searchType = "origin"
                });
            }

            // 省份建议
            var provinces = products
                .Where(p => !string.IsNullOrEmpty(p.province) && p.province.Contains(keyword))
                .Select(p => p.province)
                .Distinct()
                .Take(5)
                .ToList();

            foreach (var province in provinces)
            {
                suggestions.Add(new
                {
                    text = province,
                    type = "地区",
                    icon = "🗺️",
                    searchType = "province"
                });
            }

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                keyword = keyword,
                suggestions = suggestions
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void TestConnection(HttpContext context)
    {
        try
        {
            List<Product> products = LoadProducts();
            int totalProducts = products.Count;
            int vegetableCount = products.Count(p => p.category == "蔬菜");
            int fruitCount = products.Count(p => p.category == "水果");
            int provinceCount = products.Select(p => p.province).Distinct().Count();

            context.Response.Write("{\"success\":true,\"message\":\"Handler1.ashx 工作正常\",\"timestamp\":\"" +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"version\":\"农产品地图增强版3.0\"," +
                "\"stats\":{\"totalProducts\":" + totalProducts + ",\"vegetables\":" + vegetableCount +
                ",\"fruits\":" + fruitCount + ",\"provinces\":" + provinceCount + "}}");
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetHeatmapData(HttpContext context)
    {
        try
        {
            List<Product> products = LoadProducts();

            // 创建热力图数据点
            var heatData = new List<HeatmapPoint>();
            Random rand = new Random();

            foreach (var product in products)
            {
                // 为每个产品创建一个热力图点
                // 强度基于库存量，库存越大热力越强
                double intensity = Math.Min(1.0, product.stock / 1000.0);

                // 添加一些随机性，让热力图更有层次
                intensity += (rand.NextDouble() * 0.2 - 0.1);
                intensity = Math.Max(0.1, Math.Min(1.0, intensity));

                heatData.Add(new HeatmapPoint
                {
                    lat = product.origin_lat,
                    lng = product.origin_lng,
                    intensity = intensity
                });
            }

            var maxIntensity = heatData.Count > 0 ? heatData.Max(d => d.intensity) : 1.0;
            var minIntensity = heatData.Count > 0 ? heatData.Min(d => d.intensity) : 0.1;

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                data = heatData,
                maxIntensity = maxIntensity,
                minIntensity = minIntensity,
                pointCount = heatData.Count,
                totalProducts = products.Count
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetProvinceData(HttpContext context)
    {
        try
        {
            List<Product> products = LoadProducts();
            var provinceData = products
                .GroupBy(p => p.province)
                .Select(g => new
                {
                    province = g.Key,
                    count = g.Count(),
                    avgPrice = g.Average(p => (double)p.price),
                    totalStock = g.Sum(p => p.stock),
                    latitude = g.First().origin_lat,
                    longitude = g.First().origin_lng,
                    categories = g.GroupBy(p => p.category)
                        .Select(cg => new { name = cg.Key, count = cg.Count() })
                        .ToList()
                })
                .Where(p => !string.IsNullOrEmpty(p.province))
                .OrderByDescending(p => p.count)
                .ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                data = provinceData,
                totalProvinces = provinceData.Count
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetProvinceStats(HttpContext context)
    {
        try
        {
            string province = context.Request["province"] ?? "";
            List<Product> products = LoadProducts();

            var provinceProducts = string.IsNullOrEmpty(province)
                ? products
                : products.Where(p => p.province == province).ToList();

            var stats = new
            {
                province = province,
                totalProducts = provinceProducts.Count,
                totalStock = provinceProducts.Sum(p => p.stock),
                avgPrice = provinceProducts.Count > 0 ? provinceProducts.Average(p => (double)p.price) : 0,
                categoryCount = provinceProducts.GroupBy(p => p.category).Count(),
                originTypeCount = provinceProducts.GroupBy(p => p.origin_type).Count(),
                topProducts = provinceProducts
                    .GroupBy(p => p.product_name)
                    .Select(g => new
                    {
                        name = g.Key,
                        count = g.Count(),
                        avgPrice = g.Average(p => (double)p.price),
                        totalStock = g.Sum(p => p.stock)
                    })
                    .OrderByDescending(x => x.count)
                    .Take(5)
                    .ToList()
            };

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                stats = stats
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetDeliveryRecords(HttpContext context)
    {
        try
        {
            List<DeliveryRecord> records = LoadDeliveryRecords();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                records = records,
                count = records.Count,
                lastDelivery = records.OrderByDescending(r => r.delivery_time).FirstOrDefault()
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void GetCartData(HttpContext context)
    {
        try
        {
            string userId = context.Request["userId"] ?? "default";
            List<CartItem> cartItems = LoadCartItems(userId);

            // 加载产品信息以获取完整数据
            List<Product> products = LoadProducts();

            var cartWithDetails = cartItems.Select(item =>
            {
                var product = products.FirstOrDefault(p => p.product_id == item.product_id);
                return new
                {
                    product_id = item.product_id,
                    product_name = item.product_name,
                    origin_name = item.origin_name,
                    price = item.price,
                    quantity = item.quantity,
                    total = item.price * item.quantity,
                    stock = product?.stock ?? 0,
                    category = product?.category,
                    province = product?.province,
                    added_time = item.added_time
                };
            }).ToList();

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                userId = userId,
                items = cartWithDetails,
                count = cartItems.Count,
                totalQuantity = cartItems.Sum(item => item.quantity),
                totalAmount = cartItems.Sum(item => item.price * item.quantity)
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void AddToCart(HttpContext context)
    {
        try
        {
            string userId = context.Request["userId"] ?? "default";
            int productId = Convert.ToInt32(context.Request["productId"]);
            int quantity = Convert.ToInt32(context.Request["quantity"] ?? "1");

            // 加载产品信息
            List<Product> products = LoadProducts();
            Product product = products.FirstOrDefault(p => p.product_id == productId);

            if (product == null)
            {
                context.Response.Write("{\"success\":false,\"error\":\"产品不存在\"}");
                return;
            }

            // 检查库存
            if (product.stock < quantity)
            {
                context.Response.Write("{\"success\":false,\"error\":\"库存不足，当前库存：" + product.stock + "\"}");
                return;
            }

            // 加载购物车
            List<CartItem> cartItems = LoadCartItems(userId);

            // 检查是否已存在
            var existingItem = cartItems.FirstOrDefault(item => item.product_id == productId);
            if (existingItem != null)
            {
                // 检查更新后的库存
                if (product.stock < existingItem.quantity + quantity)
                {
                    context.Response.Write("{\"success\":false,\"error\":\"库存不足，最多可添加" + (product.stock - existingItem.quantity) + "件\"}");
                    return;
                }
                existingItem.quantity += quantity;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    product_id = productId,
                    product_name = product.product_name,
                    origin_name = product.origin_name,
                    price = product.price,
                    quantity = quantity,
                    added_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    user_id = userId
                });
            }

            // 保存购物车
            SaveCartItems(userId, cartItems);

            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(new
            {
                success = true,
                message = "已添加到购物车",
                cartCount = cartItems.Count,
                totalQuantity = cartItems.Sum(item => item.quantity),
                totalAmount = cartItems.Sum(item => item.price * item.quantity)
            });

            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void UpdateCart(HttpContext context)
    {
        try
        {
            string userId = context.Request["userId"] ?? "default";
            int productId = Convert.ToInt32(context.Request["productId"]);
            int quantity = Convert.ToInt32(context.Request["quantity"]);

            // 检查库存
            List<Product> products = LoadProducts();
            Product product = products.FirstOrDefault(p => p.product_id == productId);

            if (product == null)
            {
                context.Response.Write("{\"success\":false,\"error\":\"产品不存在\"}");
                return;
            }

            if (product.stock < quantity)
            {
                context.Response.Write("{\"success\":false,\"error\":\"库存不足，当前库存：" + product.stock + "\"}");
                return;
            }

            List<CartItem> cartItems = LoadCartItems(userId);
            var item = cartItems.FirstOrDefault(i => i.product_id == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cartItems.Remove(item);
                }
                else
                {
                    item.quantity = quantity;
                    item.added_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                SaveCartItems(userId, cartItems);

                context.Response.Write("{\"success\":true,\"message\":\"购物车已更新\",\"cartCount\":\"" + cartItems.Count + "\"}");
            }
            else
            {
                context.Response.Write("{\"success\":false,\"error\":\"购物车项不存在\"}");
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    private void ClearCart(HttpContext context)
    {
        try
        {
            string userId = context.Request["userId"] ?? "default";

            // 清空购物车
            var emptyCart = new List<CartItem>();
            SaveCartItems(userId, emptyCart);

            context.Response.Write("{\"success\":true,\"message\":\"购物车已清空\"}");
        }
        catch (Exception ex)
        {
            context.Response.Write("{\"success\":false,\"error\":\"" + CleanForJson(ex.Message) + "\"}");
        }
    }

    // 数据加载和保存方法
    private List<Product> LoadProducts()
    {
        List<Product> products = new List<Product>();

        if (!File.Exists(dataFilePath))
        {
            EnsureFilesExist();
        }

        string[] lines = File.ReadAllLines(dataFilePath, Encoding.UTF8);

        // 跳过标题行
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length >= 10)
            {
                Product product = new Product
                {
                    product_id = int.Parse(parts[0]),
                    product_name = parts[1],
                    category = parts[2],
                    price = decimal.Parse(parts[3]),
                    origin_lat = double.Parse(parts[4]),
                    origin_lng = double.Parse(parts[5]),
                    origin_name = parts[6],
                    origin_type = parts[7],
                    description = parts[8],
                    stock = int.Parse(parts[9])
                };

                // 处理省份
                if (parts.Length > 10)
                {
                    product.province = parts[10];
                }
                else
                {
                    product.province = "";
                }

                products.Add(product);
            }
        }

        return products.OrderBy(p => p.product_id).ToList();
    }

    private void SaveProducts(List<Product> products)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("product_id,product_name,category,price,origin_lat,origin_lng,origin_name,origin_type,description,stock,province");

            foreach (var product in products.OrderBy(p => p.product_id))
            {
                sb.AppendLine($"{product.product_id},{product.product_name},{product.category},{product.price}," +
                             $"{product.origin_lat},{product.origin_lng},{product.origin_name},{product.origin_type}," +
                             $"{product.description},{product.stock},{product.province}");
            }

            File.WriteAllText(dataFilePath, sb.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new Exception("保存产品数据失败: " + ex.Message);
        }
    }

    private List<DeliveryRecord> LoadDeliveryRecords()
    {
        List<DeliveryRecord> records = new List<DeliveryRecord>();

        if (!File.Exists(deliveryFilePath))
        {
            EnsureFilesExist();
            return records;
        }

        string[] lines = File.ReadAllLines(deliveryFilePath, Encoding.UTF8);

        // 跳过标题行
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length >= 6)
            {
                DeliveryRecord record = new DeliveryRecord
                {
                    delivery_time = parts[0],
                    product_id = int.Parse(parts[1]),
                    product_name = parts[2],
                    origin_name = parts[3],
                    destination = parts[4],
                    province = parts[5],
                    transport_mode = parts.Length > 6 ? parts[6] : "truck"
                };

                records.Add(record);
            }
        }

        return records.OrderByDescending(r => r.delivery_time).ToList();
    }

    private List<CartItem> LoadCartItems(string userId)
    {
        List<CartItem> items = new List<CartItem>();

        if (!File.Exists(cartFilePath))
        {
            EnsureFilesExist();
            return items;
        }

        string[] lines = File.ReadAllLines(cartFilePath, Encoding.UTF8);

        // 跳过标题行
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length >= 7 && parts[0] == userId)
            {
                CartItem item = new CartItem
                {
                    user_id = parts[0],
                    product_id = int.Parse(parts[1]),
                    product_name = parts[2],
                    origin_name = parts[3],
                    price = decimal.Parse(parts[4]),
                    quantity = int.Parse(parts[5]),
                    added_time = parts[6]
                };

                items.Add(item);
            }
        }

        return items;
    }

    private void SaveCartItems(string userId, List<CartItem> items)
    {
        try
        {
            // 先读取所有数据
            List<string> allLines = new List<string>();

            if (File.Exists(cartFilePath))
            {
                string[] lines = File.ReadAllLines(cartFilePath, Encoding.UTF8);
                allLines.AddRange(lines.Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)));
            }
            else
            {
                allLines.Add("user_id,product_id,product_name,origin_name,price,quantity,added_time");
            }

            // 移除该用户的现有数据
            allLines = allLines.Where(line => !line.StartsWith(userId + ",")).ToList();

            // 添加新数据
            foreach (var item in items)
            {
                string line = $"{userId},{item.product_id},{item.product_name},{item.origin_name},{item.price},{item.quantity},{item.added_time}";
                allLines.Add(line);
            }

            // 写回文件
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("user_id,product_id,product_name,origin_name,price,quantity,added_time");
            foreach (var line in allLines)
            {
                sb.AppendLine(line);
            }

            File.WriteAllText(cartFilePath, sb.ToString(), Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new Exception("保存购物车数据失败: " + ex.Message);
        }
    }

    private void RecordDelivery(int productId, string productName, string originName, string destination, string province, string transportMode = "truck")
    {
        try
        {
            string record = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{productId},{productName},{originName},{destination},{province},{transportMode}";
            File.AppendAllText(deliveryFilePath, record + Environment.NewLine, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new Exception("记录配送信息失败: " + ex.Message);
        }
    }

    private string GetProductIcon(string productName, string category)
    {
        var icons = new Dictionary<string, Dictionary<string, string>>
        {
            ["蔬菜"] = new Dictionary<string, string>
            {
                ["胡萝卜"] = "🥕",
                ["萝卜"] = "🥕",
                ["西红柿"] = "🍅",
                ["番茄"] = "🍅",
                ["黄瓜"] = "🥒",
                ["茄子"] = "🍆",
                ["玉米"] = "🌽",
                ["土豆"] = "🥔",
                ["青椒"] = "🫑",
                ["菠菜"] = "🥬",
                ["白菜"] = "🥬",
                ["芹菜"] = "🥬",
                ["西兰花"] = "🥦",
                ["生菜"] = "🥬",
                ["香菜"] = "🌿",
                ["韭菜"] = "🌿",
                ["大蒜"] = "🧄",
                ["生姜"] = "🧅",
                ["红薯"] = "🍠",
                ["洋葱"] = "🧅",
                ["莲藕"] = "🥔",
                ["山药"] = "🍠",
                ["苦瓜"] = "🥒",
                ["南瓜"] = "🎃",
                ["冬瓜"] = "🍈",
                ["丝瓜"] = "🥒",
                ["豇豆"] = "🫛"
            },
            ["水果"] = new Dictionary<string, string>
            {
                ["苹果"] = "🍎",
                ["香蕉"] = "🍌",
                ["橙子"] = "🍊",
                ["葡萄"] = "🍇",
                ["草莓"] = "🍓",
                ["西瓜"] = "🍉",
                ["菠萝"] = "🍍",
                ["桃子"] = "🍑",
                ["猕猴桃"] = "🥝",
                ["芒果"] = "🥭",
                ["荔枝"] = "🍈",
                ["龙眼"] = "🍈",
                ["柚子"] = "🍊",
                ["哈密瓜"] = "🍈",
                ["红枣"] = "🍒",
                ["柿子"] = "🍅",
                ["石榴"] = "🍎"
            }
        };

        if (icons.ContainsKey(category))
        {
            var categoryIcons = icons[category];
            foreach (var key in categoryIcons.Keys)
            {
                if (productName.Contains(key))
                    return categoryIcons[key];
            }
        }

        return category == "水果" ? "🍎" : "🥦";
    }

    private string CleanForJson(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
    }

    public bool IsReusable
    {
        get { return false; }
    }

    // 数据模型类
    public class Product
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string category { get; set; }
        public decimal price { get; set; }
        public double origin_lat { get; set; }
        public double origin_lng { get; set; }
        public string origin_name { get; set; }
        public string origin_type { get; set; }
        public string description { get; set; }
        public int stock { get; set; }
        public string province { get; set; }
    }

    public class HeatmapPoint
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public double intensity { get; set; }
    }

    public class DeliveryRecord
    {
        public string delivery_time { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string origin_name { get; set; }
        public string destination { get; set; }
        public string province { get; set; }
        public string transport_mode { get; set; }
    }

    public class CartItem
    {
        public string user_id { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string origin_name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string added_time { get; set; }
    }
}