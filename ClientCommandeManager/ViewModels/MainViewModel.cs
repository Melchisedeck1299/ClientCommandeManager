using ClientCommandeManager.Data;
using ClientCommandeManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientCommandeManager.ViewModels
{

    public class MainViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Commande> Commandes { get; set; }

        private Client _clientSelectionne;
        public Client ClientSelectionne
        {
            get => _clientSelectionne;
            set
            {
                _clientSelectionne = value;
                OnPropertyChanged(nameof(ClientSelectionne));
                LoadCommandes();
            }
        }
        public ICommand AjouterClientCommand { get; }
        public ICommand SupprimerClientCommand { get; }
        public ICommand AjouterCommandeCommand { get; }
        public ICommand SupprimerCommandeCommand { get; }
        public MainViewModel()
        {
            _context = new AppDbContext();
            _context.Database.EnsureCreated();


            var path = _context.Database.GetDbConnection().DataSource;
            System.Windows.MessageBox.Show("Base utilisée : " + path);

            var tables = _context.Database.ExecuteSqlRaw("SELECT name FROM sqlite_master WHERE type='table' AND name='Clients';");
            MessageBox.Show(tables > 0 ? "Table Clients trouvée ✅" : "Table Clients absente ❌");


            Clients = new ObservableCollection<Client>(_context.Clients.Include(c => c.Commandes));
            Commandes = new ObservableCollection<Commande>();

            AjouterClientCommand = new RelayCommand(AjouterClient);
            SupprimerClientCommand = new RelayCommand(SupprimerClient, () => ClientSelectionne != null);
            AjouterCommandeCommand = new RelayCommand(AjouterCommande, () => ClientSelectionne != null);
            SupprimerCommandeCommand = new RelayCommand(SupprimerCommande, () => Commandes.Count > 0);
        }

        private void LoadCommandes()
        {
            Commandes.Clear();
            if (ClientSelectionne != null)
            {
                foreach (var cmd in ClientSelectionne.Commandes)
                    Commandes.Add(cmd);
            }
        }

        private void AjouterClient()
        {
            MessageBox.Show("Ajout client déclenché ✅");

            if (string.IsNullOrWhiteSpace(NouveauNom) || string.IsNullOrWhiteSpace(NouvelEmail))
                return;

            var client = new Client { Nom = NouveauNom, Email = NouvelEmail };
            _context.Clients.Add(client);
            _context.SaveChanges();
            Clients.Add(client);

            NouveauNom = string.Empty;
            NouvelEmail = string.Empty;
        }


        private void SupprimerClient()
        {
            if (ClientSelectionne != null)
            {
                _context.Clients.Remove(ClientSelectionne);
                _context.SaveChanges();
                Clients.Remove(ClientSelectionne);
                ClientSelectionne = null;
            }
        }

        private void AjouterCommande()
        {
            if (ClientSelectionne == null || NouveauMontant <= 0)
                return;

            var commande = new Commande
            {
                DateCommande = NouvelleDate,
                Montant = NouveauMontant,
                ClientId = ClientSelectionne.Id
            };

            _context.Commandes.Add(commande);
            _context.SaveChanges();

            ClientSelectionne.Commandes.Add(commande);
            LoadCommandes();

            NouveauMontant = 0;
            NouvelleDate = DateTime.Now;
        }


        private void SupprimerCommande()
        {
            var cmd = Commandes.LastOrDefault();
            if (cmd != null)
            {
                _context.Commandes.Remove(cmd);
                _context.SaveChanges();
                Commandes.Remove(cmd);
            }
        }

        // Champs pour saisie client
        private string _nouveauNom;
        public string NouveauNom
        {
            get => _nouveauNom;
            set { _nouveauNom = value; OnPropertyChanged(nameof(NouveauNom)); }
        }

        private string _nouvelEmail;
        public string NouvelEmail
        {
            get => _nouvelEmail;
            set { _nouvelEmail = value; OnPropertyChanged(nameof(NouvelEmail)); }
        }

        // Champs pour saisie commande
        private decimal _nouveauMontant;
        public decimal NouveauMontant
        {
            get => _nouveauMontant;
            set { _nouveauMontant = value; OnPropertyChanged(nameof(NouveauMontant)); }
        }

        private DateTime _nouvelleDate = DateTime.Now;
        public DateTime NouvelleDate
        {
            get => _nouvelleDate;
            set { _nouvelleDate = value; OnPropertyChanged(nameof(NouvelleDate)); }
        }


    }
}
