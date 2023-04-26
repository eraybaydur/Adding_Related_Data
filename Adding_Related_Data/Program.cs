using Microsoft.EntityFrameworkCore;

/// <summary>
///            ### İLİŞKİSEL SENARYOLARDA VERİ EKLEME ###
/// </summary>

DataContext _context = new();

#region Bire bir ilişkisel senaryolarda veri ekleme

#region 1.Yöntem => Principal Entity Üzerinden Dependent Entity'i Ekleme

Kisi kisi = new Kisi(); // Yeni bir kişi nesnesi oluşturduk.
kisi.Ad = "Eray";       // Kişinin isim property'sine değer atadık.
kisi.Adres = new() { KisiAdres = "Marmaris / Muğla" }; // Ve adresini tanımladık.

await _context.AddAsync(kisi);
await _context.SaveChangesAsync();

/*
    *  Yukarıdaki kodlarda (Satır 13-15) yeni bir kişi nesnesi oluşturduk, ardından kişi için gerekli olan
    * propertylere değerlerini atadık. Dikkat ettiyseniz, propertylerimizden birisi bir classı temsil ediyor (Adres)
    * bu yüzden kişi nesnemizin adres property'si için de Adres sınıfından bir nesne oluşturuyoruz (Satır 15.).
    *   Kişi nesnesine, ihtiyaç duyduğu propertyleri atadıktan sonra da Context sınıfı metodlarımızla 
    * olusturdugumuz Kişi'yi veritabanına ekleyip, kaydediyoruz.(Satır 17-18.)
 */


#endregion


#region 2.Yöntem => Dependent Entity Üzerinden Principal Entity'i Ekleme

        //Burada ise 1.Yöntemin tam tersini yapıyoruz. Her iki yöntem de aynı görevi görüyor.
        //Aradaki tek fark ise şu;
        //Eğer principal entity üzerinden bir ekleme gerçekleştiriliyorsa, dependent entity
        //nesnesi verilmek zorunda değilken; dependent entity üzerinden ekleme işlemi gerçekleştiriliyorsa
        //burada principal entity'nin yani Kisi'nin nesnesine ihtiyaç vardır.

Adres adres = new()
{
    KisiAdres = "Kaş / Antalya",
    Kisi = new() { Ad = "Kadir" }
};

await _context.AddAsync(adres);
await _context.SaveChangesAsync();



#endregion

#region Entityler
class Kisi //Kisi Entity'miz (Principal Entity)
{
    public int Id { get; set; }
    public string Ad { get; set; }
    public Adres Adres { get; set; }
}

class Adres //Adres Entity'miz (Dependent Entity)
{
    public int Id { get; set; }
    public int KisiId { get; set; }
    public string KisiAdres { get; set; }
    public Kisi Kisi { get; set; }
}
#endregion

#region Context -- Fluent API ile ilişkilerin tanımlanması


public class DataContext : DbContext
{
    DbSet<Kisi> Kisiler { get; set; }
    DbSet<Adres> Adresler { get; set; }

    public DataContext()
    {

    }
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(/*Connection String*/);
        base.OnConfiguring(optionsBuilder)
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Bir adresin bir kişiye ait olduğunu ve bir kişinin de bir adresi olduğunu varsayarak oluşturduk.

        modelBuilder.Entity<Adres>()
            .HasOne(a => a.Kisi)
            .WithOne(k => k.Adres)
            .HasForeignKey<Adres>(a => a.KisiId);


        base.OnModelCreating(modelBuilder);
    }
}


#endregion


#endregion

// ##################################################################################

#region Bire Çok İlişikisel Senaryolarda Veri Ekleme



#region Entityler
class Blog //Blog Entity'miz (Principal Entity)
{
    public Blog()
    {
        Postlar = new HashSet<Post>(); //HashSet kullanım amacımız eşsiz postlar oluşturabilmek.
    }
    public int Id { get; set; }
    public string Ad { get; set; }
    public ICollection<Post> Postlar { get; set; }
}

class Post //Post Entity'miz (Dependent Entity)
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public string Baslik { get; set; }
    public Blog Blog { get; set; }
}
#endregion

#region Context -- Fluent API ile ilişkilerin tanımlanması


public class DataContext2 : DbContext
{
    DbSet<Blog> Bloglar { get; set; }
    DbSet<Post> Postlar { get; set; }

    public DataContext2()
    {

    }
    public DataContext2(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(/*Connection String*/);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Bir adresin bir kişiye ait olduğunu ve bir kişinin de bir adresi olduğunu varsayarak oluşturduk.

        modelBuilder.Entity<Blog>()
            .HasMany(a => a.Postlar)
            .WithOne(k => k.Blog)
            .HasForeignKey(a => a.BlogId);


        base.OnModelCreating(modelBuilder);
    }
}


#endregion

#endregion