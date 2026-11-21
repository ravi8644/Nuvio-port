# Nuvio Xbox Port — status técnico

## Situação atual

Este repositório recebeu a base NuvioMobile 1.0.0 fornecida para o port. A base original é um projeto Kotlin Multiplatform com implementações Android e iOS e componentes desktop. O APK recebido é Android ARM64 e não pode ser executado dentro de um pacote APPX.

O Xbox Dev Mode exige um aplicativo Windows/UWP compatível com a arquitetura do console. Um pacote APPX/MSIX é apenas o contêiner; ele não converte código Android, DEX ou bibliotecas `.so` Android em binários Windows.

## O que ainda precisa ser implementado

1. Criar um alvo Windows/Xbox real, com manifesto `Package.appxmanifest` e entrada de aplicação compatível com Xbox.
2. Portar a interface e o ciclo de vida para APIs Windows/UWP ou Xbox GDK.
3. Substituir as bibliotecas nativas Android do player (`libmpv`, `libffmpegJNI`, `libnuvio_engine` e dependências) por builds Windows compatíveis com a arquitetura escolhida.
4. Adaptar entrada de controle, navegação por gamepad, armazenamento, rede, downloads e reprodução.
5. Manter e testar a tradução `pt-rBR`.
6. Selecionar explicitamente os repositórios de plugins/add-ons autorizados. O aplicativo deve armazenar URLs de manifests/repositórios; não é correto embutir automaticamente conteúdo comunitário não selecionado.
7. Compilar no Windows com Windows SDK/Visual Studio, gerar o pacote para a arquitetura Xbox, assinar com certificado de desenvolvimento e testar no Xbox Device Portal.

## Critério de entrega

Um APPX só deve ser publicado como artefato quando o binário interno for Windows/Xbox, o manifesto declarar a arquitetura correta, a assinatura for verificável e o pacote tiver sido instalado e iniciado em um ambiente Xbox Dev Mode. Um APK renomeado, um APPX vazio ou um shell sem o player não atende ao objetivo.

## Referências

- [Xbox Media App Development Options](https://learn.microsoft.com/en-us/windows/uwp/apps-for-xbox/development-options)
- [Packaging UWP apps](https://learn.microsoft.com/en-us/windows/msix/package/packaging-uwp-apps)
- [App package architectures](https://learn.microsoft.com/en-us/windows/msix/package/device-architecture)
- [Device Portal for Xbox](https://learn.microsoft.com/en-us/windows/uwp/xbox-apps/device-portal-xbox)
- [Nuvio Plugin — How to Install & Use](https://nuvioplugin.com/how-to-use.html)
