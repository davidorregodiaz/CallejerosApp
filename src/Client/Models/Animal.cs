using System;
using System.ComponentModel.DataAnnotations;
using Client.Models.Validators;
using Microsoft.AspNetCore.Components.Forms;

namespace Client.Models;

public class Animal
{
    [Required]
    [StringLength(20,ErrorMessage = "Debe contener menos de 20 caracteres")]
    public string Name { get; set; }
    [Required]
    [Range(1,30,ErrorMessage = "La edad debe oscilar entre 1 y 30")]
    public int Age { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Debe contener menos de 50 caracteres")]
    [IsNot("---All types---")]
    public string Type { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Debe contener menos de 50 caracteres")]
    [IsNot("---All breeds---")]
    public string Breed { get; set; }
    public string OwnerId { get; set; }
    [Required]
    [StringLength(500, ErrorMessage = "Debe contener menos de 500 caracteres")]
    public string Description { get; set; }

    public IReadOnlyCollection<IBrowserFile> Images { get; set; } = new List<IBrowserFile>();

}
