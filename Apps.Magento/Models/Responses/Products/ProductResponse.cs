using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Products;

public class ProductResponse
{
    [Display("Product ID")]
    public int Id { get; set; }
    
    [Display("SKU")]
    public string Sku { get; set; }
    
    [Display("Product name")]
    public string Name { get; set; }
    
    [Display("Attribute set ID")]
    public int AttributeSetId { get; set; }
    
    [Display("Price")]
    public decimal Price { get; set; }
    
    [Display("Status")]
    public int Status { get; set; }
    
    [Display("Visibility")]
    public int Visibility { get; set; }
    
    [Display("Type ID")]
    public string TypeId { get; set; } = string.Empty;
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }
    
    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }
    
    [Display("Weight")]
    public decimal Weight { get; set; }
    
    [DefinitionIgnore]
    public ExtensionAttributes ExtensionAttributes { get; set; } = new();
    
    [DefinitionIgnore]
    public List<ProductLink> ProductLinks { get; set; } = new();
    
    [DefinitionIgnore]
    public List<object> Options { get; set; } = new();
    
    [DefinitionIgnore]
    public List<object> MediaGalleryEntries { get; set; } = new();
    
    [DefinitionIgnore]
    public List<object> TierPrices { get; set; } = new();
    
    [DefinitionIgnore]
    public List<CustomAttribute> CustomAttributes { get; set; } = new();
}

public class ExtensionAttributes
{
    [DefinitionIgnore]
    public List<int> WebsiteIds { get; set; } = new();
    
    [DefinitionIgnore]
    public List<CategoryLink> CategoryLinks { get; set; } = new();
    
    [DefinitionIgnore]
    public List<BundleProductOption> BundleProductOptions { get; set; } = new();
}

public class ProductLink
{
    [Display("SKU")]
    public string Sku { get; set; }
    
    [Display("Link type")]
    public string LinkType { get; set; }
    
    [Display("Linked product SKU")]
    public string LinkedProductSku { get; set; }
    
    [Display("Linked product type")]
    public string LinkedProductType { get; set; }
    
    [Display("Position")]
    public int Position { get; set; }
}

public class Option
{
    [Display("Product SKU")]
    public string ProductSku { get; set; }
    
    [Display("Option ID")]
    public int OptionId { get; set; }
    
    [Display("Title")]
    public string Title { get; set; }
    
    [Display("Type")]
    public string Type { get; set; }
    
    [Display("Sort order")]
    public int SortOrder { get; set; }
    
    [Display("Is require")]
    public bool IsRequire { get; set; }
    
    [Display("SKU")]
    public string Sku { get; set; }
    
    [Display("Max characters")]
    public int MaxCharacters { get; set; }
    
    [Display("Image size X")]
    public int ImageSizeX { get; set; }
    
    [Display("Image size Y")]
    public int ImageSizeY { get; set; }
    
    [DefinitionIgnore]
    public List<Value> Values { get; set; } = new();
}

public class Value
{
    [Display("Title")]
    public string Title { get; set; }
    
    [Display("Sort order")]
    public int SortOrder { get; set; }
    
    [Display("Price")]
    public decimal Price { get; set; }
    
    [Display("Price type")]
    public string PriceType { get; set; }
    
    [Display("SKU")]
    public string Sku { get; set; }
    
    [Display("Option type ID")]
    public int OptionTypeId { get; set; }
}

public class MediaGalleryEntry
{
    [Display("Media type")]
    public string MediaType { get; set; }
    
    [Display("Label")]
    public string Label { get; set; }
    
    [Display("Position")]
    public int Position { get; set; }
    
    [Display("Disabled")]
    public bool Disabled { get; set; }
    
    [Display("Types")]
    public List<string> Types { get; set; } = new();
    
    [Display("File")]
    public string File { get; set; }
    
    [Display("Content")]
    public Content Content { get; set; } = new();
}

public class Content
{
    [Display("Base64 encoded data")]
    public string Base64EncodedData { get; set; }
    
    [Display("Type")]
    public string Type { get; set; }
    
    [Display("Name")]
    public string Name { get; set; }
}

public class CustomAttribute
{
    [Display("Attribute code")]
    public string AttributeCode { get; set; }
    
    [Display("Value")]
    public object Value { get; set; }
}

public class CategoryLink
{
    [Display("Position")]
    public int Position { get; set; }
    
    [Display("Category ID")]
    public string CategoryId { get; set; }
}

public class BundleProductOption
{
    [Display("Option ID")]
    public int OptionId { get; set; }
    
    [Display("Title")]
    public string Title { get; set; }
    
    [Display("Required")]
    public bool Required { get; set; }
    
    [Display("Type")]
    public string Type { get; set; }
    
    [Display("Position")]
    public int Position { get; set; }
    
    [Display("SKU")]
    public string Sku { get; set; }
    
    [DefinitionIgnore]
    public List<ProductLink> ProductLinks { get; set; } = new();
}
