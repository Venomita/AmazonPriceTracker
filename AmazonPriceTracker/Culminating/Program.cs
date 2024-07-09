using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using HtmlAgilityPack;

namespace Culminating
{
    class Program
    {
        static Dictionary<string, decimal> productPrices = new Dictionary<string, decimal>();
        static Dictionary<string, decimal> priceThresholds = new Dictionary<string, decimal>();

        static void Main(string[] args)
        {
            // Create a timer
            var timer = new Timer(60000); // 1 minute interval
            timer.Elapsed += (sender, e) => TrackPrices();
            timer.Start();

            Console.WriteLine("Price tracker started. Press any key to exit.");

            while (true)
            {
                Console.WriteLine("\nMENU:");
                Console.WriteLine("1. Add a product to track");
                Console.WriteLine("2. Remove a product from tracking");
                Console.WriteLine("3. Display tracked products");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter the product URL to track: ");
                        string productUrl = Console.ReadLine();
                        Console.Write("Enter the price threshold: ");
                        decimal priceThreshold = decimal.Parse(Console.ReadLine());
                        AddProductToTrack(productUrl, priceThreshold);
                        break;
                    case "2":
                        Console.Write("Enter the product URL to remove from tracking: ");
                        string productToRemove = Console.ReadLine();
                        RemoveProductFromTrack(productToRemove);
                        break;
                    case "3":
                        DisplayTrackedProducts();
                        break;
                    case "4":
                        // Stop the timer and exit the application
                        timer.Stop();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void TrackPrices()
        {
            foreach (var productUrl in productPrices.Keys)
            {
                // Download HTML content
                var web = new HtmlWeb();
                var doc = web.Load(productUrl);

                // Extract price from HTML
                var priceNode = doc.DocumentNode.SelectSingleNode("//span[@id='priceblock_ourprice']");
                string priceStr = priceNode?.InnerText.Trim().Replace("$", "");
                decimal price = decimal.Parse(priceStr);

                // Store the new price
                productPrices[productUrl] = price;

                // Check if price falls below the threshold
                decimal threshold = priceThresholds[productUrl];
                if (price < threshold)
                {
                    Console.WriteLine("Price of {0} fell below the threshold: {1}", productUrl, threshold);

                    // Perform actions like sending an email notification to the user
                    SendEmailNotification(productUrl, price);
                }
            }
        }

        static void SendEmailNotification(string productUrl, decimal price)
        {
            // Implementation of sending email notification goes here
            Console.WriteLine("Email notification sent for product: {0} with price: {1}", productUrl, price);
        }

        static void AddProductToTrack(string productUrl, decimal priceThreshold)
        {
            if (!productPrices.ContainsKey(productUrl))
            {
                // Add the product URL and initial price
                productPrices.Add(productUrl, 0);
                priceThresholds.Add(productUrl, priceThreshold);
                Console.WriteLine("Product added to tracking list.");
            }
            else
            {
                Console.WriteLine("Product already being tracked.");
            }
        }

        static void RemoveProductFromTrack(string productUrl)
        {
            if (productPrices.ContainsKey(productUrl))
            {
                // Remove the product URL
                productPrices.Remove(productUrl);
                priceThresholds.Remove(productUrl);
                Console.WriteLine("Product removed from tracking list.");
            }
            else
            {
                Console.WriteLine("Product not found in the tracking list.");
            }
        }

        static void DisplayTrackedProducts()
        {
            Console.WriteLine("Tracked Products:");
            foreach (var productUrl in productPrices.Keys)
            {
                Console.WriteLine(productUrl);
            }
        }
    }
}
