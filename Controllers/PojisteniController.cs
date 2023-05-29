using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using PojistovnaApp.Data;
using PojistovnaApp.Models;

namespace PojistovnaApp.Controllers
{
    public class PojisteniController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojisteniController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int cisloStrany = 1)
        {
            // Získání seznamu pojištění a stránkování
            int zaznamuNaStranu = 5;
            var pojisteni = await _context.Pojisteni
                //.OrderBy(p => p.Id)
                .Skip((cisloStrany - 1) * zaznamuNaStranu)
                .Take(zaznamuNaStranu)
                .ToListAsync();

            // Vypočítání celkového počtu stránek
            int zaznamuCelkem = await _context.Pojisteni.CountAsync();
            int pocetStran = (int)Math.Ceiling((double)zaznamuCelkem / zaznamuNaStranu);

            // Předání dat do view
            ViewData["Pojisteni"] = pojisteni;
            ViewData["CisloStrany"] = cisloStrany;
            ViewData["PocetStran"] = pocetStran;

            // Zobrazení seznamu pojištění
            return View(pojisteni);
        }

        // GET: Pojisteni/Details/5
        // Metoda pro zobrazení detailu pojištění a jeho pojistných událostí
        public async Task<IActionResult> Details(int? id, int cisloStrany = 1)
        {
            if (id == null || _context.Pojisteni == null) // Pokud id není specifikováno nebo je Pojisteni v DB null, vrátí se chybová stránka
            {
                return NotFound();
            }

            var pojisteni = await _context.Pojisteni // Hledá se pojisteni s daným id v DB. Pokud se nenajde, vrátí se chybová stránka
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojisteni == null)
            {
                return NotFound();
            }
            // Získání seznamu pojistných událostí konkrétního pojištění a stránkování
            int zaznamuNaStranu = 3;
            var udalosti = _context.PojistneUdalosti
                .Where(p => p.PojisteniId == id)
                .Skip((cisloStrany - 1) * zaznamuNaStranu)
                .Take(zaznamuNaStranu)
                .ToList();

            // Vypočítání celkového počtu stránek
            int zaznamuCelkem = _context.PojistneUdalosti.Count(p => p.PojisteniId == id);
            int pocetStran = (int)Math.Ceiling((double)zaznamuCelkem / zaznamuNaStranu);

            // Předání dat do view
            ViewData["Udalosti"] = udalosti;
            ViewData["CisloStrany"] = cisloStrany;
            ViewData["PocetStran"] = pocetStran;

            return View(pojisteni); // Pokud se podaří najít pojištění v DB, vrátí se jeho detailní pohled.
        }

        // GET: Pojisteni/Create
        // Metoda pro zobrazení formuláře pro vytvoření pojištění (ke konkrétnímu pojistníkovi)
        public IActionResult Create(int pojistnikId)
        {
            var pojistnik = _context.Pojistenci.Find(pojistnikId); // Najdeme pojistnika s danym id v databázi
            if (pojistnik == null) // Pokud není nalezen, vrátí se chybová stránka
            {
                return NotFound();
            }
            // Uložíme do ViewBag potřebné údaje o pojistnikovi pro předání Id a zobrazení jména a přijmení v formuláři
            ViewBag.PojistnikId = pojistnik.Id;
            ViewBag.PojistnikJmeno = pojistnik.Jmeno;
            ViewBag.PojistnikPrijmeni = pojistnik.Prijmeni;

            return View(); // Vrátíme View s formulářem pro vytvoření nového pojisteni
        }

        // POST: Pojistenec/Create
        // Metoda pro zpracování formuláře pro vytvoření pojištění
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DruhPojisteni,Castka,PredmetPojisteni,DatumZacatku,DatumKonce,PojistnikId")] Pojisteni pojisteni, int pojistnikId)
        {
            if (ModelState.IsValid) // Zkontrolujeme, zda je stav modelu platný
            {
                pojisteni.PojistnikId = pojistnikId; // nastavení ID pojištěnce
                _context.Add(pojisteni); // Přidání nového pojištění do databáze
                await _context.SaveChangesAsync(); // Uložení změn v databázi
                TempData["AlertClass"] = "alert-success"; // nastavení barvy alertu
                TempData["AlertMessage"] = "Pojištění bylo uloženo."; // nastavení zprávy alertu
                return RedirectToAction("Details", "Pojistenec", new { id = pojistnikId }); // Přesměrování na detail pojištěnce
            }
            return View(pojisteni); // Návrat na formulář pro vytvoření nového pojištění v případě neúspěchu validace
        }

        // GET: Pojisteni/Edit/5
        // Metoda pro zobrazení formuláře pro editaci konkrétního pojištění s daným ID
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pojisteni == null) // Pokud id neexistuje nebo databáze Pojisteni neexistuje, vrátí se chybová stránka
            {
                return NotFound();
            }

            var pojisteni = await _context.Pojisteni.FindAsync(id); // Najde pojisteni s daným id
            if (pojisteni == null) // Pokud není nalezen žádný záznam, vrátí se chybová stránka
            {
                return NotFound();
            }

            return View(pojisteni); // Jinak se zobrazí View s daty tohoto pojisteni
        }

        // POST: Pojisteni/Edit/5
        // Metoda pro zpracování formuláře pro editaci konkrétního pojištění s daným ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DruhPojisteni,Castka,PredmetPojisteni,DatumZacatku,DatumKonce,PojistnikId")] Pojisteni pojisteni)
        {
            if (id != pojisteni.Id) // zkontrolujeme, zda-li existuje pojištění s daným id, pokud ne vrátí se chybová stránka
            {
                return NotFound();
            }

            if (ModelState.IsValid) // Zkontrolujeme, zda je stav modelu platný
            {
                try
                {
                    // Načtení původního pojištění z databáze
                    var originalPojisteni = await _context.Pojisteni.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

                    // Přiřazení původní hodnoty DruhPojisteni, pokud nebyla vybrána nová hodnota
                    if (pojisteni.DruhPojisteni == 0)
                    {
                        pojisteni.DruhPojisteni = originalPojisteni.DruhPojisteni;
                    }

                    _context.Update(pojisteni); // Aktualizace dat pojisteni v DB
                    TempData["AlertClass"] = "alert-warning"; // nastavení barvy alertu
                    TempData["AlertMessage"] = "Změny byly uloženy."; // nastavení zprávy alertu
                    await _context.SaveChangesAsync(); // Uložení změn v databázi
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PojisteniExists(pojisteni.Id)) // Kontrola, zda záznam v DB existuje a v případě neúspěchu se vrátí chybová stránka
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Pojistenec", new { id = pojisteni.PojistnikId }); // Přesměrování na detail pojištěnce
            }
            return View(pojisteni);
        }

        // GET: Pojistenec/Delete/5
        // Metoda pro zpracování formuláře pro smazání konkrétního pojištění s daným ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pojisteni == null) // Pokud ID neexistuje nebo databáze není inicializována, vrátí se chybová stránka
            {
                return NotFound();
            }

            var pojisteni = await _context.Pojisteni // Hledá se Pojisteni z databáze s konkrétním ID
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojisteni == null) // Pokud se Pojisteni v databázi nenajde, vrátí se chybová stránka
            {
                return NotFound();
            }

            return View(pojisteni); // Jinak se vrátí View s Pojistenim k smazání
        }

        // POST: Pojisteni/Delete/5
        // Metoda pro zpracovaní potvrzeného formuláře pro smazání
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pojisteni == null) // Pokud není databáze inicializována, vrátí se chybová stránka
            {
                return Problem("Entity set 'ApplicationDbContext.Pojisteni'  is null.");
            }
            var pojisteni = await _context.Pojisteni.FindAsync(id); // Vyhledání záznamu v databázi
            if (pojisteni != null) // Pokud byl záznam nalezen, tak se z databáze odstraní
            {
                _context.Pojisteni.Remove(pojisteni);
            }
            
            await _context.SaveChangesAsync(); // Uložení změn v databázi

            TempData["AlertClass"] = "alert-danger"; // nastavení barvy alertu
            TempData["AlertMessage"] = "Pojištění bylo odstraněno."; // nastavení zprávy alertu

            return RedirectToAction("Details", "Pojistenec", new { id = pojisteni.PojistnikId }); // Přesměrování na detail pojištěnce
        }

        // Metoda kontrolující, zda záznam v databázi existuje
        // Pokud je seznam Pojisteni prázdný, vrátí se false
        // V opačném případě se vrátí, zda seznam obsahuje záznam s předaným id
        private bool PojisteniExists(int id)
        {
          return (_context.Pojisteni?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
