# FIRSTMOD — CEF Boost

GTA SA / MTA:SA açıkken arka planda kalan **CEF Launcher** işlemlerini güvenli
şekilde tespit edip kapatan, koyu temalı, Türkçe masaüstü uygulaması.
.NET 8 / WPF ile yazılmıştır.

> ⚠️ **Önemli:** Bu kaynak kod bir yapay zeka sohbet ortamında (Linux,
> internet erişimi ve .NET SDK olmadan) hazırlanmıştır. Bu yüzden burada
> **derlenmiş bir .exe üretilemedi** — kodu kendi bilgisayarınızda derlemeniz
> gerekiyor. Aşağıdaki adımlar 5 dakikadan kısa sürer.

---

## 1) Gereksinimler (yalnızca derleme için, kullanıcıda GEREKMEZ)

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows, macOS veya Linux'a kurulabilir)
- Windows üzerinde derliyorsanız ekstra bir şeye gerek yok.
- Linux/macOS üzerinden **Windows için** derlemek de mümkündür (cross-publish),
  aşağıdaki komut aynen çalışır.

## 2) Tek komutla derleme (self-contained, tek dosya, 64-bit)

Proje klasörünün içinde (bu `FIRSTMOD.csproj` dosyasının bulunduğu yerde) şu
komutu çalıştırın:

```bash
dotnet publish -c Release -r win-x64
```

`.csproj` içinde `SelfContained`, `PublishSingleFile`, `RuntimeIdentifier=win-x64`
zaten tanımlı olduğu için başka bir parametre gerekmez. Derleme bitince çıktı:

```
bin\Release\net8.0-windows\win-x64\publish\FIRSTMOD.exe
```

Bu **tek `.exe` dosyası** — kullanıcı bilgisayarında ayrıca .NET kurmasına
gerek kalmadan doğrudan çift tıklayıp çalıştırabilir. Windows 10/11 64-bit
üzerinde çalışır.

## 3) (Opsiyonel) İkon eklemek

`Resources\app.ico` adında bir ikon dosyası ekleyip `FIRSTMOD.csproj`
içine şu satırı eklerseniz .exe kendi ikonuyla görünür:

```xml
<ApplicationIcon>Resources\app.ico</ApplicationIcon>
```

---

## Proje Yapısı

```
FIRSTMOD/
├─ FIRSTMOD.csproj          # Proje dosyası (self-contained/single-file ayarları)
├─ app.manifest              # Yönetici izni istemez (asInvoker)
├─ App.xaml / App.xaml.cs
├─ MainWindow.xaml / .cs     # Sol menü + sağ içerik alanı
├─ Resources/
│  └─ Theme.xaml             # Koyu tema, renkler, kart/buton stilleri, hover animasyonları
├─ Views/
│  ├─ SplashWindow.xaml      # Açılış (loading) ekranı
│  ├─ HomeView.xaml          # Ana Sayfa: CEF KAPAT kartı + sistem bilgisi kartı
│  ├─ CefOperationsView.xaml # CEF İşlemleri: tespit edilen tüm süreçlerin listesi
│  ├─ GtaMtaView.xaml        # GTA / MTA çalışma durumu
│  ├─ SystemView.xaml        # Detaylı RAM / CPU
│  └─ SettingsView.xaml      # Ayarlar / Hakkında
└─ Services/
   ├─ CefProcessService.cs   # CEF tespiti ve güvenli kapatma mantığı
   └─ SystemInfoService.cs   # RAM / CPU / oyun durumu (P/Invoke, admin gerektirmez)
```

---

## Güvenlik Tasarımı (CefProcessService.cs)

Uygulama işlemleri şu **çok katmanlı** kontrolden geçirmeden asla kapatmaz:

1. **İsim eşleşmesi** — yalnızca bilinen CEF alt-süreç adı desenleriyle
   (`cef.launcher`, `cefsharp.browsersubprocess`, isminde "cef" geçen vb.)
   eşleşen işlemler aday olur.
2. **Yol doğrulaması** — adayın çalıştırılabilir dosya yolu, MTA / GTA San
   Andreas kurulum klasörü (`Multi Theft Auto`, `MTA San Andreas`,
   `GTA San Andreas`, ya da `...\cef\...` alt klasörü) içinde olmalıdır.
   Örneğin Discord, Steam veya tarayıcı gibi başka uygulamaların CEF
   süreçleri bu filtreden geçemez.
3. **Kara liste** — `explorer`, `svchost`, `winlogon`, `lsass`, `csrss`,
   `services`, `System`, `Idle` gibi Windows'un kritik/sistem işlemleri
   isim eşleşse bile asla hedeflenmez.
4. **Beyaz liste (oyun ana işlemi)** — `GTA_SA.exe` / `Multi Theft Auto.exe`
   ana işlemi kesinlikle kapatma listesine giremez; uygulama yalnızca CEF
   alt süreçlerini hedefler, oyunu asla kapatmaz.
5. **Kendi işlemini asla hedeflemez** ve **yoluna erişilemeyen** (izin
   sorunu / korumalı) işlemleri güvenlik gereği atlar, zorla erişmeye
   çalışmaz.
6. Kapatma anında **ikinci bir doğrulama** daha yapılır (isim + kara liste),
   böylece tarama ile kapatma arasındaki zaman diliminde bir PID başka bir
   işleme "geri kullanılmış" olsa bile yanlış işlem kapatılmaz.
7. Hiçbir işlem bulunamazsa: **"CEF işlemi bulunamadı."**
   Başarılı olursa: **"CEF işlemleri başarıyla kapatıldı."**
   Başarısız olursa, nedeni (ör. "Erişim Reddedildi") kullanıcıya gösterilir.

## İzinler

`app.manifest` içinde `requestedExecutionLevel level="asInvoker"` kullanılır;
yani uygulama **yönetici (admin) izni istemez**. CEF alt süreçleri normalde
oyunla aynı kullanıcı yetkisinde çalıştığı için bu yeterlidir. Eğer bir CEF
süreci farklı bir yetki seviyesinde çalışıyorsa (nadir), uygulama bunu
kapatmayı dener ve başarısız olursa nedenini ("Erişim Reddedildi") açıkça
gösterir — sessizce görmezden gelmez ama yükseltilmiş yetki de talep etmez.
