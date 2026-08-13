# Telefonda EXE oluşturma

Bu proje GitHub Actions ile Windows üzerinde otomatik derlenebilir.

1. GitHub'da yeni bir repository oluştur.
2. ZIP içindeki **FIRSTMOD klasörünün içindeki dosyaları** repository'ye yükle.
3. `.github/workflows/build.yml` dosyası da yüklenmiş olmalı.
4. GitHub'da **Actions** sekmesine gir.
5. **Build FIRSTMOD EXE** workflow'unu seç.
6. **Run workflow** ile çalıştır.
7. İşlem tamamlanınca workflow sayfasındaki **Artifacts** bölümünden `FIRSTMOD-win-x64` dosyasını indir.
8. ZIP'i açınca `FIRSTMOD.exe` bulunur.

Not: GitHub hesabının Actions çalıştırmaya izin vermesi gerekir. Bu yöntem EXE'yi Windows runner üzerinde derler; telefonda Windows kurulumu gerekmez.
