using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Descrição: {t.description} \n" +
                                         $"Velocidade do Vento: {t.speed} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Visibilidade: {t.visibility} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n";

                        lbl_res.Text = dados_previsao;

                    }
                    else
                    {

                        lbl_res.Text = "Sem dados de Previsão";
                    }

                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }

            }
            catch (Exception ex) when (ex.Message == "CidadeNaoEncontrada")
            {
                await DisplayAlert("Cidade Não Encontrada", "Não conseguimos localizar a cidade.", "OK");
            }
            catch (Exception ex) when (ex.Message == "SemConexao")
            {
                await DisplayAlert("Sem Conexão", "Não foi possível se conectar à internet. Verifique sua rede e tente novamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

        }
    

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try 
            {
                GeolocationRequest request = new GeolocationRequest(
                    GeolocationAccuracy.Best, 
                    TimeSpan.FromSeconds(10)
                 );

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude} \n" +
                                        $"Altitude: {local.Altitude} \n" +
                                        $"Accuracy: {local.Accuracy} ";
                    lbl_coords.Text = local_disp;

                    // pega nome da cidade que esta nas coordenadas
                    GetCidade(local.Latitude, local.Longitude);
                }
                else 
                {
                    lbl_coords.Text = "Nenhuma localização.";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta.", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx) 
            {
                await DisplayAlert("Erro: Localização Desabilitada.", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão Negada.", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Ocorreu um erro inesperado.", ex.Message, "OK");
            }
        }

        private async void GetCidade(double lat, double lon)
        {
            try
            {

                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro: Não foi possível obter a cidade.", ex.Message, "OK");
            }
        }
    }
}