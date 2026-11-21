# Nuvio Xbox — cliente UWP experimental

Este projeto é um cliente UWP separado, configurado para x64 e sideload no Xbox Dev Mode. Ele não contém o APK Android nem tenta convertê-lo.

## Recursos desta versão inicial

- Interface de TV em português do Brasil, navegável por controle/teclado.
- Adição, validação, persistência e remoção de URLs de manifests Stremio.
- Consulta básica de catálogos de filmes/séries e streams por título.
- Reprodução de URLs diretas HTTP/HTTPS aceitas pelo MediaPlayer do Windows.
- Busca automática nos add-ons configurados por legendas em português, priorizando pt-BR, e sobreposição temporizada para legendas SRT/WebVTT.
- Link para o diretório comunitário de add-ons indicado pelo usuário.
- Pipeline GitHub Actions em Windows para compilar e assinar um APPX de teste x64.

## Limitações importantes

Isto ainda não é um port completo do NuvioMobile. O motor Nuvio de plugins WASM/QuickJS, os plugins nativos, a integração Android libmpv/Media3 e o MPVKit iOS não rodam neste UWP. O cliente inicial só entende manifests Stremio e URLs diretas que o MediaPlayer do Xbox aceite. Streams P2P/torrent, headers específicos, DRM, reprodução por libmpv, login/contas Nuvio, sincronização de perfis/progresso e legendas injetadas como faixas nativas ainda não foram portados. A sobreposição SRT/WebVTT é experimental e precisa de teste real no Xbox.

O app não instala automaticamente todos os repositórios do site comunitário: ele aceita manifests Stremio informados pelo usuário e abre o diretório para consulta. Repositórios WASM de plugins Nuvio não são equivalentes a manifests Stremio e ainda não são compatíveis com este cliente. O app tenta preferir pt-BR quando um add-on retorna essa informação, mas disponibilidade e sincronização dependem do serviço e do formato.

## Build no GitHub Actions

Faça push de alterações em `xbox/**` para `port/xbox-devmode` ou execute manualmente o workflow **Build Nuvio Xbox APPX**. O job Windows cria um certificado temporário, compila o cliente para x64, assina o APPX e publica o pacote e o certificado `.cer` como artefato da execução. O certificado vale somente para aquele build; confie o `.cer` correspondente no ambiente de teste conforme o fluxo do Xbox Device Portal. Não reutilize esse certificado em futuras versões.

O sucesso de compilação não comprova compatibilidade de cada codec, add-on, resolução ou legenda. O teste final precisa ser feito no Xbox Series em Dev Mode.
