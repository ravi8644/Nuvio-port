# Nuvio Xbox — cliente UWP experimental

Este projeto é um cliente UWP x64 separado, preparado para o fluxo de sideload no Xbox Dev Mode. Ele não contém o APK Android nem tenta convertê-lo.

## Recursos da versão inicial

- Interface de TV em português do Brasil, navegável por controle/teclado.
- Adição, validação, persistência e remoção de URLs de manifests Stremio.
- Leitura de catálogos de filmes e séries e consulta de streams por título.
- Reprodução de URLs diretas HTTP/HTTPS aceitas pelo MediaPlayer do Windows.
- Link para o diretório comunitário de add-ons indicado pelo usuário.
- Pipeline GitHub Actions em Windows para compilar e assinar um APPX de teste x64.

## Limitações importantes

Isto ainda não é um port completo do NuvioMobile. O motor Nuvio de plugins WASM/QuickJS, os plugins nativos, a integração Android libmpv/Media3 e o MPVKit iOS não rodam neste UWP. O cliente inicial só entende o protocolo de manifests Stremio e URLs diretas de mídia que o MediaPlayer do Xbox aceite. Streams P2P/torrent, cabeçalhos específicos, DRM, reprodução por libmpv, login/contas Nuvio, sincronização de perfis/progresso e faixas de legendas externas ainda não foram portados ou testados.

O app não instala automaticamente todos os add-ons da internet. Cada URL é escolhida pelo usuário; isso evita instalar fontes comunitárias não confiáveis e respeita configurações, licenças e credenciais de cada serviço. A interface está localizada em pt-BR, mas o idioma das faixas de áudio/legenda depende dos próprios streams. Não há ainda suporte garantido a legendas externas.

## Build no GitHub Actions

Faça push de alterações em `xbox/**` para a branch `port/xbox-devmode` ou execute manualmente o workflow `Build Nuvio Xbox APPX`. O job Windows cria um certificado de desenvolvimento temporário, compila o projeto para x64, assina o APPX e publica os arquivos `.appx` e `.cer` como artefato da execução. O certificado é temporário para aquele build; o `.cer` correspondente precisa ser confiado no ambiente de teste conforme o fluxo do Xbox Device Portal. Não reutilize esse certificado em futuras versões.

O APPX só deve ser considerado validado depois de instalado e iniciado em um Xbox em Dev Mode. O sucesso de compilação do workflow não comprova compatibilidade de cada codec, add-on, resolução, legenda ou serviço.
