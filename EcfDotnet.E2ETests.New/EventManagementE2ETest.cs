using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EcfDotnet.E2ETests.New
{
    [TestClass]
    public class EventManagementE2ETest
    {
        [TestMethod]
        public async Task CreerEvenementEtVerifier()
        {
            // Créer une instance de Playwright
            using var playwright = await Playwright.CreateAsync();
            
            // Lancer un navigateur
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                SlowMo = 100 
            });
            
            
            var page = await browser.NewPageAsync();
            
            try
            {
                // Étape 1: Naviguer directement vers la page de liste des événements
                await page.GotoAsync("http://localhost:5017/Evenement/Index");
                await page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "01-page-liste-evenements.png",
                    FullPage = true 
                });
                
                Console.WriteLine("Page de liste des événements chargée, capture d'écran prise");
                
                // Étape 2: Cliquer sur le bouton "Créer un nouvel événement"
                await page.ClickAsync("text=Créer un nouvel événement");
                
                // Attendre que la page de création soit chargée
                await page.WaitForSelectorAsync("form[action='/Evenement/Create']", new PageWaitForSelectorOptions { Timeout = 30000 });
                
                await page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "02-page-creation-evenement.png",
                    FullPage = true 
                });
                
                Console.WriteLine("Page de création d'événement chargée, capture d'écran prise");
                
                // Étape 3: Remplir le formulaire de création d'événement
                var eventName = "Conférence Test " + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                await page.FillAsync("input[name='Nom']", eventName);
                await page.FillAsync("input[name='Description']", "Description de test pour l'événement");
                
                // Formater les dates correctement
                var tomorrow = DateTime.Now.AddDays(1);
                var dayAfterTomorrow = DateTime.Now.AddDays(2);
                
                await page.FillAsync("input[name='DateDebut']", tomorrow.ToString("yyyy-MM-dd"));
                await page.FillAsync("input[name='DateFin']", dayAfterTomorrow.ToString("yyyy-MM-dd"));
                await page.FillAsync("input[name='Localisation']", "Paris, France");
                
                await page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "03-formulaire-evenement-rempli.png",
                    FullPage = true 
                });
                
                Console.WriteLine("Formulaire rempli, capture d'écran prise");
                
                // Étape 4: Soumettre le formulaire et attendre la réponse
                var submitButton = await page.QuerySelectorAsync("button[type='submit']");
                
                // Créer une tâche pour attendre la navigation
                var waitForNavigationTask = page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                // Cliquer sur le bouton
                await submitButton.ClickAsync();
                
                // Attendre que la navigation soit terminée
                await waitForNavigationTask;
                
                // Vérifier si nous sommes sur la page d'index ou si une erreur s'est produite
                var currentUrl = page.Url;
                Console.WriteLine($"URL après soumission : {currentUrl}");
                
                await page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "04-apres-soumission.png",
                    FullPage = true 
                });
                
                // Si nous sommes redirigés vers la page d'index ou la page Evenement (qui est aussi la liste)
                if (currentUrl.Contains("/Evenement/Index") || currentUrl.EndsWith("/Evenement"))
                {
                    // Attendre que la page se charge complètement
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    
                    // Rechercher l'événement dans la liste
                    var eventExists = await page.IsVisibleAsync($"text={eventName}");
                    Assert.IsTrue(eventExists, "L'événement n'a pas été créé correctement");
                    
                    Console.WriteLine("Test terminé avec succès - Événement créé et vérifié");
                }
                else
                {
                    // Si nous ne sommes pas sur la page d'index, il y a peut-être eu une erreur
                    var pageContent = await page.ContentAsync();
                    Console.WriteLine("Contenu de la page après soumission :");
                    Console.WriteLine(pageContent.Substring(0, Math.Min(500, pageContent.Length)) + "...");
                    
                    Assert.Fail($"Redirection inattendue vers {currentUrl}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du test : {ex.Message}");
                Console.WriteLine($"Stack trace : {ex.StackTrace}");
                await page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "erreur.png",
                    FullPage = true 
                });
                throw;
            }
        }
    }
}
