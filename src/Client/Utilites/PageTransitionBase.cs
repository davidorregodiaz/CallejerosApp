
namespace Client.Utilites;

// PageTransitionBase.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public class PageTransitionBase : ComponentBase, IDisposable
{
    [Inject] protected NavigationManager NavigationManager { get; set; }

    protected int AnimationClass { get; set; } = 0;
    protected bool IsLeaving { get; set; }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    private async void HandleLocationChanged(object sender, LocationChangedEventArgs e)
    {
        // Trigger fade-out al cambiar de ruta
        IsLeaving = true;
        AnimationClass = 0;
        
        // Esperar a que la animación complete
        await Task.Delay(500); // Duración igual a la animación CSS
        
        // Resetear estado para próxima página
        IsLeaving = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Trigger fade-in después del render inicial
            await Task.Delay(50); // Pequeño delay para activar la animación
            AnimationClass = 1;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }
}
