using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hasta
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public bool isInsured { get; set; }
    public List<Randevu> RandevuList { get; set; }
}
