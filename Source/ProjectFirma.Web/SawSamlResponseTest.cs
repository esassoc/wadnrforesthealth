using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectFirma.Web.Models;

namespace ProjectFirma.Web
{

    [TestFixture]
    public class SawSamlResponseTest
    {
        /// <summary>
        /// To update these, log in locally and the AccountController logs the saw saml response, so you can copy from the log file.
        /// After updating these, don't forget to update the testDatTime in the TestSawSamlResponseDateValidity method.
        /// </summary>
        private string _sampleSawSamlPrettyPrinted = @"<samlp:Response xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"" xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Destination=""https://wadnrforesthealth.localhost.projectfirma.com/Account/SAWPost"" ID=""FIMRSP_ace3530-019b-1c75-bad2-b3380158f201"" IssueInstant=""2025-12-11T00:27:24Z"" Version=""2.0"">
  <saml:Issuer Format=""urn:oasis:names:tc:SAML:2.0:nameid-format:entity"">https://test-secureaccess.wa.gov/FIM2/sps/sawidp/saml20</saml:Issuer>
  <samlp:Status>
    <samlp:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"">
    </samlp:StatusCode>
  </samlp:Status>
  <saml:Assertion ID=""Assertion-uuidace350e-019b-1602-809f-b3380158f201"" IssueInstant=""2025-12-11T00:27:24Z"" Version=""2.0"">
    <saml:Issuer Format=""urn:oasis:names:tc:SAML:2.0:nameid-format:entity"">https://test-secureaccess.wa.gov/FIM2/sps/sawidp/saml20</saml:Issuer>
    <ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" Id=""uuidace351b-019b-1198-abca-b3380158f201"">
      <ds:SignedInfo>
        <ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"">
        </ds:CanonicalizationMethod>
        <ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"">
        </ds:SignatureMethod>
        <ds:Reference URI=""#Assertion-uuidace350e-019b-1602-809f-b3380158f201"">
          <ds:Transforms>
            <ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"">
            </ds:Transform>
            <ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"">
              <ec:InclusiveNamespaces xmlns:ec=""http://www.w3.org/2001/10/xml-exc-c14n#"" PrefixList=""saml xs xsi"">
              </ec:InclusiveNamespaces>
            </ds:Transform>
          </ds:Transforms>
          <ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"">
          </ds:DigestMethod>
          <ds:DigestValue>AO+2zzvOsVKDGajRFVZYrIHsNBDuS9UpNeNYEZ3Ku0Y=</ds:DigestValue>
        </ds:Reference>
      </ds:SignedInfo>
      <ds:SignatureValue>AL7/dsAWV7qN6WvHd8yN9wbzzwOhoOohSiuF/YAhJu18/AsrUlBWhQF4zw1x08RumksAv7Pw1lTs4M5YSUL6fIPNMUE4zqMBtBEMmp5TaklTRzqjKGBdQ7TmVcJHmVIP70C9NJQKSluF9XyGIoOWSVkO4t7Beg4hqvo1psqnEjIWbonxCgmjj/dx/3aAkK1lvOhi0oPNcBrhpcPIiO2XTKrW8h1o+8DT+9mKs/WWN1mRSSWyRKbYyEETn1H22i23DcJbc84St4owMG7e7/w8TUlg4mpCZUmLM+WjEaRc1gM5nOlzb64/0T/fR7R7uYIg/VzGEpRUtRcT0X8Be1w6fg==</ds:SignatureValue>
      <ds:KeyInfo>
        <ds:X509Data>
          <ds:X509Certificate>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</ds:X509Certificate>
        </ds:X509Data>
      </ds:KeyInfo>
    </ds:Signature>
    <saml:Subject>
      <saml:NameID Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress"">DP4MT6TV3ZT7M-3QT5ZL6DM-DD7WV4ZZ8D-1FZ3DD4VZ4</saml:NameID>
      <saml:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"">
        <saml:SubjectConfirmationData NotOnOrAfter=""2025-12-11T00:28:24Z"" Recipient=""https://wadnrforesthealth.localhost.projectfirma.com/Account/SAWPost"">
        </saml:SubjectConfirmationData>
      </saml:SubjectConfirmation>
    </saml:Subject>
    <saml:Conditions NotBefore=""2025-12-11T00:26:24Z"" NotOnOrAfter=""2025-12-11T00:28:24Z"">
      <saml:AudienceRestriction>
        <saml:Audience>https://wadnrforesthealth.localhost.projectfirma.com</saml:Audience>
      </saml:AudienceRestriction>
    </saml:Conditions>
    <saml:AuthnStatement AuthnInstant=""2025-12-11T00:27:24Z"" SessionIndex=""uuided446012-e3eb-457a-b1d7-7269dcd04097"" SessionNotOnOrAfter=""2025-12-11T01:27:24Z"">
      <saml:AuthnContext>
        <saml:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:Password</saml:AuthnContextClassRef>
      </saml:AuthnContext>
    </saml:AuthnStatement>
    <saml:AttributeStatement>
      <saml:Attribute Name=""name"" NameFormat=""urn:oasis:names:tc:SAML:2.0:assertion"">
        <saml:AttributeValue xsi:type=""xs:string"">tom kamin</saml:AttributeValue>
      </saml:Attribute>
      <saml:Attribute Name=""level"" NameFormat=""urn:oasis:names:tc:SAML:2.0:assertion"">
        <saml:AttributeValue xsi:type=""xs:string"">1</saml:AttributeValue>
      </saml:Attribute>
      <saml:Attribute Name=""groups"" NameFormat=""urn:oasis:names:tc:SAML:2.0:assertion"">
        <saml:AttributeValue xsi:type=""xs:string"">f3_users</saml:AttributeValue>
      </saml:Attribute>
      <saml:Attribute Name=""user"" NameFormat=""urn:oasis:names:tc:SAML:2.0:assertion"">
        <saml:AttributeValue xsi:type=""xs:string"">tom.kamin</saml:AttributeValue>
      </saml:Attribute>
      <saml:Attribute Name=""email"" NameFormat=""urn:oasis:names:tc:SAML:2.0:assertion"">
        <saml:AttributeValue xsi:type=""xs:string"">tom.kamin@sitkatech.com</saml:AttributeValue>
      </saml:Attribute>
      <saml:Attribute Name=""guid"" NameFormat=""urn:oasis:names:tc:SAML:2.0:assertion"">
        <saml:AttributeValue xsi:type=""xs:string"">DP4MT6TV3ZT7M-3QT5ZL6DM-DD7WV4ZZ8D-1FZ3DD4VZ4</saml:AttributeValue>
      </saml:Attribute>
    </saml:AttributeStatement>
  </saml:Assertion>
</samlp:Response>";

        private string _sampleSawSamlBase64Encoded = @"PHNhbWxwOlJlc3BvbnNlIHhtbG5zOnNhbWw9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDph
c3NlcnRpb24iIHhtbG5zOnNhbWxwPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6cHJvdG9j
b2wiIHhtbG5zOnhzPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYSIgeG1sbnM6eHNp
PSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYS1pbnN0YW5jZSIgRGVzdGluYXRpb249
Imh0dHBzOi8vd2FkbnJmb3Jlc3RoZWFsdGgubG9jYWxob3N0LnByb2plY3RmaXJtYS5jb20vQWNj
b3VudC9TQVdQb3N0IiBJRD0iRklNUlNQX2FjZTM1MzAtMDE5Yi0xYzc1LWJhZDItYjMzODAxNThm
MjAxIiBJc3N1ZUluc3RhbnQ9IjIwMjUtMTItMTFUMDA6Mjc6MjRaIiBWZXJzaW9uPSIyLjAiPjxz
YW1sOklzc3VlciBGb3JtYXQ9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpuYW1laWQtZm9y
bWF0OmVudGl0eSI+aHR0cHM6Ly90ZXN0LXNlY3VyZWFjY2Vzcy53YS5nb3YvRklNMi9zcHMvc2F3
aWRwL3NhbWwyMDwvc2FtbDpJc3N1ZXI+PHNhbWxwOlN0YXR1cz48c2FtbHA6U3RhdHVzQ29kZSBW
YWx1ZT0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOnN0YXR1czpTdWNjZXNzIj48L3NhbWxw
OlN0YXR1c0NvZGU+PC9zYW1scDpTdGF0dXM+PHNhbWw6QXNzZXJ0aW9uIElEPSJBc3NlcnRpb24t
dXVpZGFjZTM1MGUtMDE5Yi0xNjAyLTgwOWYtYjMzODAxNThmMjAxIiBJc3N1ZUluc3RhbnQ9IjIw
MjUtMTItMTFUMDA6Mjc6MjRaIiBWZXJzaW9uPSIyLjAiPjxzYW1sOklzc3VlciBGb3JtYXQ9InVy
bjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpuYW1laWQtZm9ybWF0OmVudGl0eSI+aHR0cHM6Ly90
ZXN0LXNlY3VyZWFjY2Vzcy53YS5nb3YvRklNMi9zcHMvc2F3aWRwL3NhbWwyMDwvc2FtbDpJc3N1
ZXI+PGRzOlNpZ25hdHVyZSB4bWxuczpkcz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxk
c2lnIyIgSWQ9InV1aWRhY2UzNTFiLTAxOWItMTE5OC1hYmNhLWIzMzgwMTU4ZjIwMSI+PGRzOlNp
Z25lZEluZm8+PGRzOkNhbm9uaWNhbGl6YXRpb25NZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3
LnczLm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiPjwvZHM6Q2Fub25pY2FsaXphdGlvbk1ldGhv
ZD48ZHM6U2lnbmF0dXJlTWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8w
NC94bWxkc2lnLW1vcmUjcnNhLXNoYTI1NiI+PC9kczpTaWduYXR1cmVNZXRob2Q+PGRzOlJlZmVy
ZW5jZSBVUkk9IiNBc3NlcnRpb24tdXVpZGFjZTM1MGUtMDE5Yi0xNjAyLTgwOWYtYjMzODAxNThm
MjAxIj48ZHM6VHJhbnNmb3Jtcz48ZHM6VHJhbnNmb3JtIEFsZ29yaXRobT0iaHR0cDovL3d3dy53
My5vcmcvMjAwMC8wOS94bWxkc2lnI2VudmVsb3BlZC1zaWduYXR1cmUiPjwvZHM6VHJhbnNmb3Jt
PjxkczpUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzEwL3htbC1l
eGMtYzE0biMiPjxlYzpJbmNsdXNpdmVOYW1lc3BhY2VzIHhtbG5zOmVjPSJodHRwOi8vd3d3Lncz
Lm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiIFByZWZpeExpc3Q9InNhbWwgeHMgeHNpIj48L2Vj
OkluY2x1c2l2ZU5hbWVzcGFjZXM+PC9kczpUcmFuc2Zvcm0+PC9kczpUcmFuc2Zvcm1zPjxkczpE
aWdlc3RNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGVuYyNz
aGEyNTYiPjwvZHM6RGlnZXN0TWV0aG9kPjxkczpEaWdlc3RWYWx1ZT5BTysyenp2T3NWS0RHYWpS
RlZaWXJJSHNOQkR1UzlVcE5lTllFWjNLdTBZPTwvZHM6RGlnZXN0VmFsdWU+PC9kczpSZWZlcmVu
Y2U+PC9kczpTaWduZWRJbmZvPjxkczpTaWduYXR1cmVWYWx1ZT5BTDcvZHNBV1Y3cU42V3ZIZDh5
Tjl3Ynp6d09ob09vaFNpdUYvWUFoSnUxOC9Bc3JVbEJXaFFGNHp3MXgwOFJ1bWtzQXY3UHcxbFRz
NE01WVNVTDZmSVBOTVVFNHpxTUJ0QkVNbXA1VGFrbFRSenFqS0dCZFE3VG1WY0pIbVZJUDcwQzlO
SlFLU2x1RjlYeUdJb09XU1ZrTzR0N0JlZzRocXZvMXBzcW5FaklXYm9ueENnbWpqL2R4LzNhQWtL
MWx2T2hpMG9QTmNCcmhwY1BJaU8yWFRLclc4aDFvKzhEVCs5bUtzL1dXTjFtUlNTV3lSS2JZeUVF
VG4xSDIyaTIzRGNKYmM4NFN0NG93TUc3ZTcvdzhUVWxnNG1wQ1pVbUxNK1dqRWFSYzFnTTVuT2x6
YjY0LzBUL2ZSN1I3dVlJZy9WekdFcFJVdFJjVDBYOEJlMXc2Zmc9PTwvZHM6U2lnbmF0dXJlVmFs
dWU+PGRzOktleUluZm8+PGRzOlg1MDlEYXRhPjxkczpYNTA5Q2VydGlmaWNhdGU+TUlJRy9UQ0NC
ZVdnQXdJQkFnSVFCWk92bFhrcDBPZnhWaXRnWW54Mk1UQU5CZ2txaGtpRzl3MEJBUXNGQURCWk1R
c3dDUVlEVlFRR0V3SlZVekVWTUJNR0ExVUVDaE1NUkdsbmFVTmxjblFnU1c1ak1UTXdNUVlEVlFR
REV5cEVhV2RwUTJWeWRDQkhiRzlpWVd3Z1J6SWdWRXhUSUZKVFFTQlRTRUV5TlRZZ01qQXlNQ0JE
UVRFd0hoY05NalV3TlRFeU1EQXdNREF3V2hjTk1qWXdOVEV4TWpNMU9UVTVXakNCZ1RFTE1Ba0dB
MVVFQmhNQ1ZWTXhFekFSQmdOVkJBZ1RDbGRoYzJocGJtZDBiMjR4RURBT0JnTlZCQWNUQjA5c2VX
MXdhV0V4S0RBbUJnTlZCQW9USDFkaGMyaHBibWQwYjI0Z1ZHVmphRzV2Ykc5bmVTQlRiMngxZEds
dmJuTXhJVEFmQmdOVkJBTVRHSFJsYzNRdGMyVmpkWEpsWVdOalpYTnpMbmRoTG1kdmRqQ0NBU0l3
RFFZSktvWklodmNOQVFFQkJRQURnZ0VQQURDQ0FRb0NnZ0VCQUxCT3pvRGpPNzI2ZFlPVk90U0Vr
Ly9ZbUV6aXRFQzYwT3JObXQxRGU5NVpGSEg3SXllMkpvbW1Td0htMXlKRXMwaFVBK3VGcTY1cEZE
L0tlRi9nZktVWko5anpYWGZ3SEdyb3VMQ0hhaGJiV0hNYlJEK284R1FkQkRQZyt0aTk2Zmpwd0dq
S3VjUDk1bWU1RW4wbnkxbW00WDFWUmJidDdFdk1pcWVyZDh1Z1V2YnNGL0hHcm12ck52M1RiVE1U
K0NwaHNwV0tCclIzRDZHNktqRXZuaFhlVkY2VWtRN2dIMXlWazNsSXY0dldjbFp4dXpYalJ1SFM1
RGRDdVZ4aDN2dEJ3UU9Va0k5RjV6Mmx0U0lVVEpONjcvZzRZR0xmY1o0YUMwOHlxR08rc0JudjJZ
U3MrdmpXc002a1I4RGRIekQ4OStBM0tGWW91clVHMks4VE03MENBd0VBQWFPQ0E1WXdnZ09TTUI4
R0ExVWRJd1FZTUJhQUZIU0ZnTUJteDk4MzNzKzlLVGVxQXgyKzdjMFhNQjBHQTFVZERnUVdCQlNC
MXArbk1kNjRTYlIwZFdjU1lTT3B0L2pLNVRBakJnTlZIUkVFSERBYWdoaDBaWE4wTFhObFkzVnla
V0ZqWTJWemN5NTNZUzVuYjNZd1BnWURWUjBnQkRjd05UQXpCZ1puZ1F3QkFnSXdLVEFuQmdnckJn
RUZCUWNDQVJZYmFIUjBjRG92TDNkM2R5NWthV2RwWTJWeWRDNWpiMjB2UTFCVE1BNEdBMVVkRHdF
Qi93UUVBd0lGb0RBZEJnTlZIU1VFRmpBVUJnZ3JCZ0VGQlFjREFRWUlLd1lCQlFVSEF3SXdnWjhH
QTFVZEh3U0JsekNCbERCSW9FYWdSSVpDYUhSMGNEb3ZMMk55YkRNdVpHbG5hV05sY25RdVkyOXRM
MFJwWjJsRFpYSjBSMnh2WW1Gc1J6SlVURk5TVTBGVFNFRXlOVFl5TURJd1EwRXhMVEV1WTNKc01F
aWdScUJFaGtKb2RIUndPaTh2WTNKc05DNWthV2RwWTJWeWRDNWpiMjB2UkdsbmFVTmxjblJIYkc5
aVlXeEhNbFJNVTFKVFFWTklRVEkxTmpJd01qQkRRVEV0TVM1amNtd3dnWWNHQ0NzR0FRVUZCd0VC
Qkhzd2VUQWtCZ2dyQmdFRkJRY3dBWVlZYUhSMGNEb3ZMMjlqYzNBdVpHbG5hV05sY25RdVkyOXRN
RkVHQ0NzR0FRVUZCekFDaGtWb2RIUndPaTh2WTJGalpYSjBjeTVrYVdkcFkyVnlkQzVqYjIwdlJH
bG5hVU5sY25SSGJHOWlZV3hITWxSTVUxSlRRVk5JUVRJMU5qSXdNakJEUVRFdE1TNWpjblF3REFZ
RFZSMFRBUUgvQkFJd0FEQ0NBWUFHQ2lzR0FRUUIxbmtDQkFJRWdnRndCSUlCYkFGcUFIY0FscGRr
djFWWWw2MzNRNGRvTndoQ2QrbndPdFgycFBNMmJrYWtQdy9LcWNZQUFBR1d4WDdOdndBQUJBTUFT
REJHQWlFQXpYakNwMUFDcFJsdlY4VUNkems2TlAzUFViMmgyY0lDczhkUTVsMnRzR2NDSVFDdDZD
ZnQvUWVqRXpZZXZxbDBGd2hvT3RjRURtNXJveis4RFFoOThJMVBud0IzQUdRUnhHeWtFdXluaVJ5
aUFpNEF2S3RQS0FmVUhqVW5xK3IrMVFQSmZjM3dBQUFCbHNWK3piSUFBQVFEQUVnd1JnSWhBUElq
ZzU2WFJMMDhvNURtbDNZTW9NYml5MVVKUWhOWWN0eTkvbEtPZTRlK0FpRUExallDdVRnQWJsNm0r
THFQYlpYeklOZkxXanNIZjF4ZVhLQ2k0SlZGZEFBQWRnQkpuSnRwM2gxODdQdzIzczJIWkthNFc2
OEtoNEFaMFZWUysrbnJLZDM0d3dBQUFaYkZmczNLQUFBRUF3QkhNRVVDSUFvYUFRbGJPM0EycGVJ
c0g2cGdaZG9sMm82UDVZZGRheGNnaEFFdTVCcEFBaUVBODRPSGNVRFcxUHlxZHVpOS9nRmV3aStL
U09sdFJjWkw3ZktyOGhYdzBONHdEUVlKS29aSWh2Y05BUUVMQlFBRGdnRUJBRTk4Y2h0RFBEeFdp
OUlUeHUxSmpTOUhCTWZZbkZNMVV1OHVOOXFjRXdTbW9BM1MraUFZNFE5S3dqSENSVlhCSnF1S0JI
aXpZTXpVcGtSa3Awb0tVYm82c2g3ZUhrbkNDUHZ0SWxieFBHU2pndFpWNEE0UjN4RWZNelhhM2FZ
aDNYR3FJMzd6MS94UUYwVWtEZi9QM0E5QWo2MnpUWUdDUk5JOHJsUndGRkUvVXMvUDZoMUxQSzJP
aTd1bFc3SWlFVGtQVHBBRlNqNjNlWGNWczJMTUZCZmtLZWxsN01RdlRRTDJKRmJEaVlWb3JvMEhC
aWxybmRabWNSOXZaeERJTWwvS2JaaWpjOUw3TFdYcWp0RGRib3A3SWpzRXRpT2RXQTcwdnBDWmZM
cFRRL1pKeGdURzRWaWU4RDNEdWtpc2IrdXlQcVFhVU9aaHRodWpaWVpWUVV3PTwvZHM6WDUwOUNl
cnRpZmljYXRlPjwvZHM6WDUwOURhdGE+PC9kczpLZXlJbmZvPjwvZHM6U2lnbmF0dXJlPjxzYW1s
OlN1YmplY3Q+PHNhbWw6TmFtZUlEIEZvcm1hdD0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6MS4x
Om5hbWVpZC1mb3JtYXQ6ZW1haWxBZGRyZXNzIj5EUDRNVDZUVjNaVDdNLTNRVDVaTDZETS1ERDdX
VjRaWjhELTFGWjNERDRWWjQ8L3NhbWw6TmFtZUlEPjxzYW1sOlN1YmplY3RDb25maXJtYXRpb24g
TWV0aG9kPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6Y206YmVhcmVyIj48c2FtbDpTdWJq
ZWN0Q29uZmlybWF0aW9uRGF0YSBOb3RPbk9yQWZ0ZXI9IjIwMjUtMTItMTFUMDA6Mjg6MjRaIiBS
ZWNpcGllbnQ9Imh0dHBzOi8vd2FkbnJmb3Jlc3RoZWFsdGgubG9jYWxob3N0LnByb2plY3RmaXJt
YS5jb20vQWNjb3VudC9TQVdQb3N0Ij48L3NhbWw6U3ViamVjdENvbmZpcm1hdGlvbkRhdGE+PC9z
YW1sOlN1YmplY3RDb25maXJtYXRpb24+PC9zYW1sOlN1YmplY3Q+PHNhbWw6Q29uZGl0aW9ucyBO
b3RCZWZvcmU9IjIwMjUtMTItMTFUMDA6MjY6MjRaIiBOb3RPbk9yQWZ0ZXI9IjIwMjUtMTItMTFU
MDA6Mjg6MjRaIj48c2FtbDpBdWRpZW5jZVJlc3RyaWN0aW9uPjxzYW1sOkF1ZGllbmNlPmh0dHBz
Oi8vd2FkbnJmb3Jlc3RoZWFsdGgubG9jYWxob3N0LnByb2plY3RmaXJtYS5jb208L3NhbWw6QXVk
aWVuY2U+PC9zYW1sOkF1ZGllbmNlUmVzdHJpY3Rpb24+PC9zYW1sOkNvbmRpdGlvbnM+PHNhbWw6
QXV0aG5TdGF0ZW1lbnQgQXV0aG5JbnN0YW50PSIyMDI1LTEyLTExVDAwOjI3OjI0WiIgU2Vzc2lv
bkluZGV4PSJ1dWlkZWQ0NDYwMTItZTNlYi00NTdhLWIxZDctNzI2OWRjZDA0MDk3IiBTZXNzaW9u
Tm90T25PckFmdGVyPSIyMDI1LTEyLTExVDAxOjI3OjI0WiI+PHNhbWw6QXV0aG5Db250ZXh0Pjxz
YW1sOkF1dGhuQ29udGV4dENsYXNzUmVmPnVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDphYzpj
bGFzc2VzOlBhc3N3b3JkPC9zYW1sOkF1dGhuQ29udGV4dENsYXNzUmVmPjwvc2FtbDpBdXRobkNv
bnRleHQ+PC9zYW1sOkF1dGhuU3RhdGVtZW50PjxzYW1sOkF0dHJpYnV0ZVN0YXRlbWVudD48c2Ft
bDpBdHRyaWJ1dGUgTmFtZT0ibmFtZSIgTmFtZUZvcm1hdD0idXJuOm9hc2lzOm5hbWVzOnRjOlNB
TUw6Mi4wOmFzc2VydGlvbiI+PHNhbWw6QXR0cmlidXRlVmFsdWUgeHNpOnR5cGU9InhzOnN0cmlu
ZyI+dG9tIGthbWluPC9zYW1sOkF0dHJpYnV0ZVZhbHVlPjwvc2FtbDpBdHRyaWJ1dGU+PHNhbWw6
QXR0cmlidXRlIE5hbWU9ImxldmVsIiBOYW1lRm9ybWF0PSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FN
TDoyLjA6YXNzZXJ0aW9uIj48c2FtbDpBdHRyaWJ1dGVWYWx1ZSB4c2k6dHlwZT0ieHM6c3RyaW5n
Ij4xPC9zYW1sOkF0dHJpYnV0ZVZhbHVlPjwvc2FtbDpBdHRyaWJ1dGU+PHNhbWw6QXR0cmlidXRl
IE5hbWU9Imdyb3VwcyIgTmFtZUZvcm1hdD0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmFz
c2VydGlvbiI+PHNhbWw6QXR0cmlidXRlVmFsdWUgeHNpOnR5cGU9InhzOnN0cmluZyI+ZjNfdXNl
cnM8L3NhbWw6QXR0cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48c2FtbDpBdHRyaWJ1dGUg
TmFtZT0idXNlciIgTmFtZUZvcm1hdD0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmFzc2Vy
dGlvbiI+PHNhbWw6QXR0cmlidXRlVmFsdWUgeHNpOnR5cGU9InhzOnN0cmluZyI+dG9tLmthbWlu
PC9zYW1sOkF0dHJpYnV0ZVZhbHVlPjwvc2FtbDpBdHRyaWJ1dGU+PHNhbWw6QXR0cmlidXRlIE5h
bWU9ImVtYWlsIiBOYW1lRm9ybWF0PSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6YXNzZXJ0
aW9uIj48c2FtbDpBdHRyaWJ1dGVWYWx1ZSB4c2k6dHlwZT0ieHM6c3RyaW5nIj50b20ua2FtaW5A
c2l0a2F0ZWNoLmNvbTwvc2FtbDpBdHRyaWJ1dGVWYWx1ZT48L3NhbWw6QXR0cmlidXRlPjxzYW1s
OkF0dHJpYnV0ZSBOYW1lPSJndWlkIiBOYW1lRm9ybWF0PSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FN
TDoyLjA6YXNzZXJ0aW9uIj48c2FtbDpBdHRyaWJ1dGVWYWx1ZSB4c2k6dHlwZT0ieHM6c3RyaW5n
Ij5EUDRNVDZUVjNaVDdNLTNRVDVaTDZETS1ERDdXVjRaWjhELTFGWjNERDRWWjQ8L3NhbWw6QXR0
cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48L3NhbWw6QXR0cmlidXRlU3RhdGVtZW50Pjwv
c2FtbDpBc3NlcnRpb24+PC9zYW1scDpSZXNwb25zZT4=";

        [Test]
        [Description("For this test we want the base64 and sample pretty printed to be the same so we can debug the tests more easily")]
        public void SampleBase64AndPrettyPrintAreTheSame()
        {
            var sawSamlResponse = SawSamlResponse.CreateFromBase64String(_sampleSawSamlBase64Encoded);
            Assert.That(sawSamlResponse.GetSamlAsPrettyPrintXml(), Is.EqualTo(_sampleSawSamlPrettyPrinted));

        }

        /// <summary>
        /// testDateTime needs to be updated with a time between the SawSamlResponse's valid time, between the NotBefore and NotOnOrAfter
        /// </summary>
        [Test]
        public void TestSawSamlResponseDateValidity()
        {
            var sawSamlResponse = SawSamlResponse.CreateFromString(_sampleSawSamlPrettyPrinted);
            var testDateTime = DateTimeOffset.Parse("2025-12-11T00:27:24Z");
            Assert.That(sawSamlResponse.IsResponseStillWithinValidTimePeriod(testDateTime), Is.True, "Should be valid date");
            Assert.That(sawSamlResponse.IsResponseStillWithinValidTimePeriod(testDateTime.AddMinutes(5)), Is.False, "Should be invalid date too old");
        }

        [Test]
        public void FieldParsingShouldBeAccurate()
        {
            var sawSamlResponse = SawSamlResponse.CreateFromString(_sampleSawSamlPrettyPrinted);
            var sawSamlResponse1 = SawSamlResponse.CreateFromBase64String(_sampleSawSamlBase64Encoded);

            var listOfResponses = new List<SawSamlResponse>() { sawSamlResponse, sawSamlResponse1 };

            foreach (var samlResponse in listOfResponses)
            {
                Assert.That(samlResponse.GetEmail(), Is.EqualTo("tom.kamin@sitkatech.com"));
                Assert.That(samlResponse.GetFirstName(), Is.EqualTo("tom"));
                Assert.That(samlResponse.GetIssuer(), Is.EqualTo("https://test-secureaccess.wa.gov/FIM2/sps/sawidp/saml20"));
                Assert.That(samlResponse.GetLastName(), Is.EqualTo("kamin"));
                Assert.That(samlResponse.GetWhichSawAuthenticator().AuthenticatorFullName, Is.EqualTo(Authenticator.SAWTEST.AuthenticatorFullName));
                Assert.That(samlResponse.GetRoleGroups(), Is.EquivalentTo(new List<string> { "f3_users" }));// , "DNR-ForestHealthTrackerQA-USER-REG", "DNR-ForestHealthTrackerQA-DEFAULT_UMG-USER-REG"
            }
        }

        [Test]
        public void ValidationShouldMostlyWork()
        {
            var sawSamlResponse = SawSamlResponse.CreateFromBase64String(_sampleSawSamlBase64Encoded);
            sawSamlResponse.IsValid(out string userDisplayableValidationErrorMessage);
            Assert.That(userDisplayableValidationErrorMessage, Is.EqualTo("Current time is past the expiration time for the SAW xml response."), "Should get most of the way through validation");
        }
    }
}