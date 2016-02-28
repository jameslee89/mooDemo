using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkenLabs.Market.Core
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRating> AccountRatings { get; set; }
        public DbSet<Sku> Skus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductColour> ProductColours { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeOffer> TradeOffers { get; set; }
        public DbSet<ViewCount> ViewCounts { get; set; }
        public DbSet<SmsConfirmationCode> SmsConfirmationCodes { get; set; }
        public DbSet<TradeReview> TradeReviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<NotificationSmsDelivery> NotificationSmsDeliveries { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
    }
}
