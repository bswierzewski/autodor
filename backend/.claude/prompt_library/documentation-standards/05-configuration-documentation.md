### 🎯 RULE: Configuration and Business Impact

**Rule:** When documenting configuration classes and properties, always explain the **business impact** of changing a value.

**Example:**
```csharp
/// <summary>
/// Maximum time to wait for the external API response before timing out.
/// Default: 30 seconds.
///
/// Business Impact: Higher values improve reliability on slow networks but may
/// negatively affect user experience. Lower values may cause timeouts during peak hours.
/// </summary>
public TimeSpan RequestTimeout { get; set; }
```