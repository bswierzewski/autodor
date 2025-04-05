using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Common;

namespace Infrastructure.Services.Generator;

public class HtmlGeneratorService : IHtmlGeneratorService
{
    public string Generate(Contractor contractor, Order order)
    {
        string base64Logo = ResourcesConverter.ConvertResourceToBase64("Infrastructure.Resources.Images.Logo.png");

        var htmlContent = $@"
        <html>
            <head>
                <meta charset='utf-8' />
                <title>WZ (Wydanie Zewnętrzne)</title>
                <style>
                    body {{ font-family: Arial; }}
                    h1 {{ text-align: center; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #000; padding: 8px; text-align: left; }}
                    th {{ background-color: #f2f2f2; }}
                    .header {{ margin-bottom: 20px; position: relative; }}
                    .logo {{ position: absolute; top: 0; right: 0; width: 150px; height: auto; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <img src='data:image/png;base64,{base64Logo}' alt='Company Logo' class='logo' />
                    <h1>Wydanie Zewnętrzne</h1>
                    <div><strong>Data:</strong> {order.Date:dd-MM-yyyy}</div>
                    <div><strong>Numer Zlecenia:</strong> {order.Number}</div>
                    <div><strong>Klient:</strong> {contractor.Name}</div>
                    <div><strong>Adres:</strong> {contractor.Street}, {contractor.ZipCode} {contractor.City}</div>
                    <div><strong>NIP:</strong> {contractor.NIP}</div>
                    <div><strong>Email:</strong> {contractor.Email}</div>
                </div>
                <table>
                    <thead>
                        <tr>
                            <th>Nazwa</th>
                            <th>Ilość</th>
                            <th>Wartość netto</th>
                            <th>Wartość VAT</th>
                            <th>Wartość Brutto</th>
                        </tr>
                    </thead>
                    <tbody>";

        foreach (var item in order.Items)
        {
            htmlContent += $@"
            <tr>
                <td>{item.PartName}</td>
                <td>{item.Quantity}</td>
                <td>{item.TotalPrice:F2} zł</td>
                <td>{Math.Round(item.TotalPrice * 0.23M, 2):F2} zł</td>
                <td>{Math.Round(item.TotalPrice * 1.23M, 2):F2} zł</td>
            </tr>";
        }

        htmlContent += @"
                    </tbody>
                </table>
            </body>
        </html>";

        return htmlContent;
    }
}