using Terminal.Gui;

namespace InventarViewerApp.UI
{
    public class SettingsDialog : Dialog
    {
        public string ClientApiFqdn { get; private set; }
        public string ClientApiPort { get; private set; }
        public string MongoDbFqdn { get; private set; }
        public string MongoDbPort { get; private set; }
        public string PgSqlDbFqdn { get; private set; }
        public string PgSqlDbPort { get; private set; }
        public string PgSqlDbName { get; private set; } // Neue Property
        
        public bool Canceled { get; private set; } = true;

        public SettingsDialog(
            string currentApiFqdn = "localhost", string currentApiPort = "80",
            string currentMongoFqdn = "localhost", string currentMongoPort = "27017",
            string currentPgSqlFqdn = "localhost", string currentPgSqlPort = "5432",
            string currentPgSqlDbName = "postgres") // Neuer Parameter mit Default
            : base("Einstellungen", 50, 24) // Höhe etwas angepasst
        {
            var apiLabel = new Label("Client API FQDN:") { X = 1, Y = 1 };
            var apiFqdnText = new TextField(currentApiFqdn) { X = 20, Y = 1, Width = 25 };
            var apiPortLabel = new Label("Port:") { X = 1, Y = 2 };
            var apiPortText = new TextField(currentApiPort) { X = 20, Y = 2, Width = 10 };

            var mongoLabel = new Label("MongoDB FQDN:") { X = 1, Y = 4 };
            var mongoFqdnText = new TextField(currentMongoFqdn) { X = 20, Y = 4, Width = 25 };
            var mongoPortLabel = new Label("Port:") { X = 1, Y = 5 };
            var mongoPortText = new TextField(currentMongoPort) { X = 20, Y = 5, Width = 10 };

            var pgSqlLabel = new Label("PgSQL DB FQDN:") { X = 1, Y = 7 };
            var pgSqlFqdnText = new TextField(currentPgSqlFqdn) { X = 20, Y = 7, Width = 25 };
            var pgSqlPortLabel = new Label("Port:") { X = 1, Y = 8 };
            var pgSqlPortText = new TextField(currentPgSqlPort) { X = 20, Y = 8, Width = 10 };
            var pgSqlDbLabel = new Label("PgSQL DB Name:") { X = 1, Y = 9 }; // Neues Label
            var pgSqlDbText = new TextField(currentPgSqlDbName) { X = 20, Y = 9, Width = 25 }; // Neues TextField

            Add(apiLabel, apiFqdnText, apiPortLabel, apiPortText);
            Add(mongoLabel, mongoFqdnText, mongoPortLabel, mongoPortText);
            Add(pgSqlLabel, pgSqlFqdnText, pgSqlPortLabel, pgSqlPortText, pgSqlDbLabel, pgSqlDbText); // Textfeld hinzugefügt

            var btnOk = new Button("OK", true);
            btnOk.Clicked += () =>
            {
                ClientApiFqdn = apiFqdnText.Text.ToString();
                ClientApiPort = apiPortText.Text.ToString();
                MongoDbFqdn = mongoFqdnText.Text.ToString();
                MongoDbPort = mongoPortText.Text.ToString();
                PgSqlDbFqdn = pgSqlFqdnText.Text.ToString();
                PgSqlDbPort = pgSqlPortText.Text.ToString();
                PgSqlDbName = pgSqlDbText.Text.ToString(); // Wert übernehmen
                
                Canceled = false;
                Application.RequestStop();
            };

            var btnCancel = new Button("Abbrechen");
            btnCancel.Clicked += () =>
            {
                Canceled = true;
                Application.RequestStop();
            };

            AddButton(btnOk);
            AddButton(btnCancel);
        }
    }
}