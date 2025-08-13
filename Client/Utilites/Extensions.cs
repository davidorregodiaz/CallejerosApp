using System;
using Microsoft.JSInterop;

namespace Client.Utilites;

public static class Extensions
{

    
    // Work with local and session storage
    public static async ValueTask SetToLocalStorage(this IJSRuntime js, string key, string value) => await js.InvokeVoidAsync("localStorage.setItem", key, value);

    public static async ValueTask<string?> GetFromLocalStorage(this IJSRuntime js,string key, string? defaultValue = null) => await js.InvokeAsync<string>("localStorage.getItem", key) ?? defaultValue;

    public static async ValueTask RemoveFromLocalStorage(this IJSRuntime js,string key) => await js.InvokeVoidAsync("localStorage.removeItem", key);

    public static async ValueTask SetToSessionStorage(this IJSRuntime js,string key, string value) => await js.InvokeVoidAsync("sessionStorage.setItem", key, value);

    public static async ValueTask<string?> GetFromSessionStorage(this IJSRuntime js,string key, string? defaultValue = null) => await js.InvokeAsync<string>("sessionStorage.getItem", key) ?? defaultValue;

    public static async ValueTask RemoveFromSessionStorage(this IJSRuntime js,string key) => await js.InvokeVoidAsync("sessionStorage.removeItem", key); 
}
