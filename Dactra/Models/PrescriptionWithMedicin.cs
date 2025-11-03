using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class PrescriptionWithMedicin
    {
        [Key, Column(Order = 0)]
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int MedicinesId { get; set; }
        public Medicines Medicines { get; set; } = null!;
        public int frequencyPerDay { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
