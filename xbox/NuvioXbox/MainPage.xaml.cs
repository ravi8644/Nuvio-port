using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NuvioXbox
{
    public sealed partial class MainPage : Page
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly ObservableCollection<AddonEntry> _addons = new ObservableCollection<AddonEntry>();
        private readonly ObservableCollection<CatalogEntry> _catalog = new ObservableCollection<CatalogEntry>();
        private readonly ObservableCollection<StreamEntry> _streams = new ObservableCollection<StreamEntry>();
        private string _currentType = "movie";
        private CatalogEntry _selected;

        public MainPage()
        {
            InitializeComponent();
            AddonList.ItemsSource = _addons;
            CatalogList.ItemsSource = _catalog;
            StreamList.ItemsSource = _streams;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var saved = ApplicationData.Current.LocalSettings.Values["addon-manifests"] as string;
                if (!string.IsNullOrWhiteSpace(saved))
                {
                    var urls = JsonArray.Parse(saved);
                    foreach (var value in urls)
                        await LoadAddonAsync(value.GetString(), false);
                }
                if (_addons.Count == 0)
                    StatusText.Text = "Adicione um manifesto Stremio. A lista começa vazia para você escolher suas próprias fontes.";
                else
                    StatusText.Text = "Add-ons carregados. Escolha Filmes ou Séries para consultar os catálogos.";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Não foi possível restaurar os add-ons: " + ex.Message;
            }
        }

        private async void AddAddon_Click(object sender, RoutedEventArgs e)
        {
            var url = ManifestUrlBox.Text == null ? "" : ManifestUrlBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                StatusText.Text = "Cole a URL completa do manifesto.";
                return;
            }
            await LoadAddonAsync(url, true);
        }

        private async void OpenAddonLibrary_Click(object sender, RoutedEventArgs e)
        {
            var opened = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://nuvioplugin.com/addons.html"));
            if (!opened)
                StatusText.Text = "Não foi possível abrir o diretório. Acesse nuvioplugin.com/addons.html em outro dispositivo e copie um manifesto Stremio compatível.";
        }

        private async Task LoadAddonAsync(string url, bool report)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri) || (uri.Scheme != "https" && uri.Scheme != "http"))
            {
                StatusText.Text = "URL inválida. Use um endereço HTTP ou HTTPS de manifesto.";
                return;
            }
            if (_addons.Any(a => string.Equals(a.ManifestUrl, url, StringComparison.OrdinalIgnoreCase)))
            {
                StatusText.Text = "Esse manifesto já está adicionado.";
                return;
            }
            try
            {
                StatusText.Text = "Validando manifesto…";
                var json = await Client.GetStringAsync(uri);
                var manifest = JsonObject.Parse(json);
                var name = ReadString(manifest, "name", "Add-on sem nome");
                var addon = new AddonEntry { Name = name, ManifestUrl = url, Manifest = manifest };
                _addons.Add(addon);
                SaveAddons();
                ManifestUrlBox.Text = "";
                StatusText.Text = report ? "Add-on adicionado. Atualize os catálogos para consultar o conteúdo." : "Manifesto restaurado: " + name;
            }
            catch (Exception ex)
            {
                StatusText.Text = "Falha ao ler o manifesto: " + ex.Message;
            }
        }

        private void SaveAddons()
        {
            var urls = new JsonArray();
            foreach (var addon in _addons)
                urls.Add(JsonValue.CreateStringValue(addon.ManifestUrl));
            ApplicationData.Current.LocalSettings.Values["addon-manifests"] = urls.Stringify();
        }

        private void RemoveAddon_Click(object sender, RoutedEventArgs e)
        {
            var selected = AddonList.SelectedItem as AddonEntry;
            if (selected == null)
            {
                StatusText.Text = "Selecione um add-on para remover.";
                return;
            }
            _addons.Remove(selected);
            SaveAddons();
            StatusText.Text = "Add-on removido deste dispositivo.";
        }

        private void Movies_Click(object sender, RoutedEventArgs e)
        {
            _currentType = "movie";
            SectionTitle.Text = "Catálogos de filmes";
            RefreshCatalogs();
        }

        private void Series_Click(object sender, RoutedEventArgs e)
        {
            _currentType = "series";
            SectionTitle.Text = "Catálogos de séries";
            RefreshCatalogs();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) { RefreshCatalogs(); }

        private async void RefreshCatalogs()
        {
            _catalog.Clear();
            _streams.Clear();
            _selected = null;
            StatusText.Text = "Consultando catálogos…";
            var count = 0;
            foreach (var addon in _addons)
            {
                try
                {
                    var catalogs = addon.Manifest.GetNamedArray("catalogs", new JsonArray());
                    foreach (var catalog in catalogs)
                    {
                        if (count >= 80) break;
                        if (catalog.ValueType != JsonValueType.Object) continue;
                        var item = catalog.GetObject();
                        var type = ReadString(item, "type", "");
                        var id = ReadString(item, "id", "");
                        if (type != _currentType || string.IsNullOrWhiteSpace(id)) continue;
                        var endpoint = BuildEndpoint(addon.ManifestUrl, "catalog/" + Uri.EscapeDataString(type) + "/" + Uri.EscapeDataString(id) + ".json");
                        var response = JsonObject.Parse(await Client.GetStringAsync(endpoint));
                        var metas = response.GetNamedArray("metas", new JsonArray());
                        foreach (var value in metas)
                        {
                            if (value.ValueType != JsonValueType.Object) continue;
                            var meta = value.GetObject();
                            var metaId = ReadString(meta, "id", "");
                            if (string.IsNullOrWhiteSpace(metaId)) continue;
                            _catalog.Add(new CatalogEntry
                            {
                                Id = metaId,
                                Type = ReadString(meta, "type", type),
                                Name = ReadString(meta, "name", "Sem título"),
                                Description = ReadString(meta, "description", ReadString(meta, "releaseInfo", "")),
                                SourceName = addon.Name,
                                ManifestUrl = addon.ManifestUrl
                            });
                            count++;
                            if (count >= 80) break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StatusText.Text = "Alguns catálogos não responderam (" + addon.Name + "): " + ex.Message;
                }
            }
            if (count == 0)
                StatusText.Text = "Nenhum item foi retornado. Confira se o add-on oferece catálogos para " + (_currentType == "movie" ? "filmes" : "séries") + ".";
            else
                StatusText.Text = count + " itens carregados. Selecione um título para buscar streams.";
        }

        private async void Catalog_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as CatalogEntry;
            if (selected == null) return;
            _selected = selected;
            SelectedTitle.Text = selected.Name;
            SelectedDescription.Text = selected.Description;
            _streams.Clear();
            StatusText.Text = "Buscando streams nos add-ons configurados…";
            foreach (var addon in _addons)
            {
                try
                {
                    var endpoint = BuildEndpoint(addon.ManifestUrl, "stream/" + Uri.EscapeDataString(selected.Type) + "/" + Uri.EscapeDataString(selected.Id) + ".json");
                    var response = JsonObject.Parse(await Client.GetStringAsync(endpoint));
                    var streams = response.GetNamedArray("streams", new JsonArray());
                    foreach (var value in streams)
                    {
                        if (value.ValueType != JsonValueType.Object) continue;
                        var stream = value.GetObject();
                        var url = ReadString(stream, "url", "");
                        var title = ReadString(stream, "title", ReadString(stream, "name", "Stream"));
                        var direct = Uri.TryCreate(url, UriKind.Absolute, out var streamUri) && (streamUri.Scheme == "http" || streamUri.Scheme == "https");
                        _streams.Add(new StreamEntry
                        {
                            Name = title,
                            Url = direct ? url : "",
                            Detail = direct ? addon.Name + " · reprodução direta" : addon.Name + " · formato não suportado nesta versão",
                            IsDirect = direct
                        });
                    }
                }
                catch
                {
                    // Add-ons often do not implement streams or do not support this title.
                }
            }
            StatusText.Text = _streams.Count == 0
                ? "Nenhum stream direto foi encontrado. Streams torrent/P2P e formatos específicos do Nuvio ainda não são suportados."
                : _streams.Count + " streams retornados. Selecione um stream direto HTTP/HTTPS para reproduzir.";
        }

        private void Stream_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selected = e.ClickedItem as StreamEntry;
            if (selected == null) return;
            if (!selected.IsDirect)
            {
                StatusText.Text = "Este stream não é um URL direto HTTP/HTTPS e não pode ser reproduzido por este port inicial.";
                return;
            }
            try
            {
                Player.Source = MediaSource.CreateFromUri(new Uri(selected.Url));
                StatusText.Text = "Reproduzindo: " + selected.Name;
            }
            catch (Exception ex)
            {
                StatusText.Text = "Não foi possível iniciar a reprodução: " + ex.Message;
            }
        }

        private static Uri BuildEndpoint(string manifestUrl, string relativePath)
        {
            var marker = manifestUrl.LastIndexOf("/manifest.json", StringComparison.OrdinalIgnoreCase);
            var prefix = marker >= 0
                ? manifestUrl.Substring(0, marker + 1)
                : manifestUrl.Substring(0, manifestUrl.LastIndexOf('/') + 1);
            return new Uri(prefix + relativePath);
        }

        private static string ReadString(JsonObject obj, string key, string fallback)
        {
            IJsonValue value;
            if (obj != null && obj.TryGetValue(key, out value) && value.ValueType == JsonValueType.String)
                return value.GetString();
            return fallback;
        }
    }

    public sealed class AddonEntry
    {
        public string Name { get; set; }
        public string ManifestUrl { get; set; }
        public JsonObject Manifest { get; set; }
    }

    public sealed class CatalogEntry
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SourceName { get; set; }
        public string ManifestUrl { get; set; }
    }

    public sealed class StreamEntry
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Detail { get; set; }
        public bool IsDirect { get; set; }
    }
}
