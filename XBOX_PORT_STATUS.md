# Nuvio Xbox Port — status técnico

## Estado atual

Há um cliente UWP x64 separado em `xbox/NuvioXbox`, voltado ao sideload no Xbox Dev Mode. A base NuvioMobile original foi mantida no repositório como referência; o APK Android não é usado como binário do APPX.

O protótipo oferece interface de TV pt-BR navegável por controle, gerenciamento de URLs de manifests Stremio, consulta simples de catálogos e streams, reprodução de URLs diretas HTTP/HTTPS pelo player do Windows e busca automática experimental de legendas em português com preferência por pt-BR em formatos SRT/WebVTT. A pipeline GitHub Actions compila e assina um APPX de teste x64.

## Diferença para o NuvioMobile completo

O Nuvio original não declara um alvo JVM desktop compilável; as quatro fontes `desktopMain` são stubs. O motor Android libmpv/Media3, os plugins WASM/QuickJS e as integrações nativas não rodam automaticamente no UWP. Este cliente não tem logins/sincronização Nuvio, motor de plugins WASM, streams P2P/torrent, DRM, reprodução por mpv, headers específicos ou faixa de legendas integrada ao pipeline de mídia. A legenda é renderizada como sobreposição e ainda precisa de teste em console.

O app não instala automaticamente todos os repositórios comunitários. Ele aceita manifests Stremio fornecidos pelo usuário; repositórios Nuvio WASM não são equivalentes e ainda não são compatíveis com este cliente. Add-ons e legendas disponíveis dependem dos serviços configurados.

## Geração e validação

O workflow `.github/workflows/xbox-appx.yml` usa runner Windows, gera um certificado de desenvolvimento temporário, compila para x64 e publica o APPX assinado e o certificado `.cer` como artefatos. Um build verde não equivale a teste em console. O pacote precisa ser instalado, aberto e exercitado em um Xbox Series em Dev Mode; codecs, streams, controle e legendas devem ser validados nesse hardware.

## Referências

- [Xbox Media App Development Options](https://learn.microsoft.com/en-us/windows/uwp/apps-for-xbox/development-options)
- [Getting started with UWP app development on Xbox](https://learn.microsoft.com/en-us/windows/uwp/xbox-apps/getting-started)
- [App package architectures](https://learn.microsoft.com/en-us/windows/msix/package/device-architecture)
- [Packaging UWP apps](https://learn.microsoft.com/en-us/windows/msix/package/packaging-uwp-apps)
- [Device Portal for Xbox](https://learn.microsoft.com/en-us/windows/uwp/xbox-apps/device-portal-xbox)
- [Nuvio Plugin — How to Install & Use](https://nuvioplugin.com/how-to-use.html)
