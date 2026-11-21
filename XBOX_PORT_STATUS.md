# Nuvio Xbox Port — status técnico

## Estado atual

O repositório contém agora um cliente UWP separado em `xbox/NuvioXbox`, orientado a x64 para o Xbox Dev Mode. Ele não usa o APK Android como binário interno. A base NuvioMobile original segue disponível no repositório para consulta e portabilidade futura.

O cliente experimental tem interface de TV em pt-BR, navegação por controle, cadastro/persistência de URLs de manifests Stremio, consulta básica a catálogos e streams, e reprodução de URLs diretas HTTP/HTTPS pelo `MediaPlayer` do Windows. Foi criado um workflow Windows para compilar e assinar um APPX de desenvolvimento.

## O que ainda não é paridade com NuvioMobile

A arquitetura compartilhada do Nuvio tem muitas interfaces de plataforma, e o repositório não declara um alvo JVM desktop compilável. O motor Android `libmpv`/Media3, os plugins WASM/QuickJS e as integrações nativas não rodam automaticamente no UWP. O cliente inicial não implementa logins/sincronização Nuvio, motores dos plugins Nuvio, streams P2P/torrent, reprodução por mpv, DRM, headers de autenticação arbitrários nem legendas externas. A UI está em português do Brasil, mas seleção automática de faixas de legenda/áudio e legendas externas ainda não foram portadas.

O app não instala automaticamente todos os repositórios do site comunitário: ele aceita manifests Stremio informados pelo usuário e abre o diretório para consulta. Repositórios WASM de plugins Nuvio não são equivalentes a manifests Stremio e ainda não são compatíveis com este cliente.

## Geração e validação do APPX

O workflow `.github/workflows/xbox-appx.yml` compila para x64 em um runner Windows, gera um certificado de desenvolvimento temporário, assina o APPX e publica o pacote e o certificado `.cer` como artefato da execução. O `.cer` corresponde somente àquela compilação. O pacote ainda precisa ser testado no Xbox Series em Dev Mode; sucesso de build sozinho não comprova instalação, compatibilidade de codecs, add-ons ou legendas.

## Referências

- [Xbox Media App Development Options](https://learn.microsoft.com/en-us/windows/uwp/apps-for-xbox/development-options)
- [Getting started with UWP app development on Xbox](https://learn.microsoft.com/en-us/windows/uwp/xbox-apps/getting-started)
- [App package architectures](https://learn.microsoft.com/en-us/windows/msix/package/device-architecture)
- [Packaging UWP apps](https://learn.microsoft.com/en-us/windows/msix/package/packaging-uwp-apps)
- [Device Portal for Xbox](https://learn.microsoft.com/en-us/windows/uwp/xbox-apps/device-portal-xbox)
- [Nuvio Plugin — How to Install & Use](https://nuvioplugin.com/how-to-use.html)
