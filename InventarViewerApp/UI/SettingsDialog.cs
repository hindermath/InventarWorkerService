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
        public string PgSqlDbName { get; private set; }
        public string PgSqlUser { get; private set; }
        public string PgSqlPassword { get; private set; }

        public bool Canceled { get; private set; } = true;

        public SettingsDialog(
            string currentApiFqdn = "localhost", string currentApiPort = "80",
            string currentMongoFqdn = "localhost", string currentMongoPort = "27017",
            string currentPgSqlFqdn = "localhost", string currentPgSqlPort = "5432",
            string currentPgSqlDbName = "postgres",
            string currentPgSqlUser = "", string currentPgSqlPassword = "")
            : base("Einstellungen", 50, 28)
        {
            var apiLabel = new Label("Client API FQDN:") { X = 1, Y = 1 };
            var apiFqdnText = new TextField(currentApiFqdn) { X = 20, Y = 1, Width = 25 };            var apiPortLabel = new Label("Port:") { X = 1, Y = 2 };
            var apiPortText = new TextField(currentApiPort) { X = 20, Y = 2, Width = 10 };

            var mongoLabel = new Label("MongoDB FQDN:") { X = 1, Y = 4 };
            var mongoFqdnText = new TextField(currentMongoFqdn) { X = 20, Y = 4, Width = 25 };
            var mongoPortLabel = new Label("Port:") { X = 1, Y = 5 };
            var mongoPortText = new TextField(currentMongoPort) { X = 20, Y = 5, Width = 10 };

            var pgSqlLabel = new Label("PgSQL DB FQDN:") { X = 1, Y = 7 };
            var pgSqlFqdnText = new TextField(currentPgSqlFqdn) { X = 20, Y = 7, Width = 25 };
            var pgSqlPortLabel = new Label("Port:") { X = 1, Y = 8 };
            var pgSqlPortText = new TextField(currentPgSqlPort) { X = 20, Y = 8, Width = 10 };
            var pgSqlDbLabel = new Label("PgSQL DB Name:") { X = 1, Y = 9 };
            var pgSqlDbText = new TextField(currentPgSqlDbName) { X = 20, Y = 9, Width = 25 };

            var pgSqlAuthCheck = new CheckBox("PgSQL Auth", !string.IsNullOrEmpty(currentPgSqlUser)) { X = 1, Y = 10 };

            var pgSqlUserLabel = new Label("User:") { X = 1, Y = 11 };
            var pgSqlUserText = new TextField(currentPgSqlUser) { X = 20, Y = 11, Width = 25 };

            var pgSqlPassLabel = new Label("Passwort:") { X = 1, Y = 12 };
            var pgSqlPassText = new TextField(currentPgSqlPassword) { X = 20, Y = 12, Width = 25, Secret = true };

            // Initialer Status basierend auf Checkbox
            pgSqlUserText.Enabled = pgSqlAuthCheck.Checked;
            pgSqlPassText.Enabled = pgSqlAuthCheck.Checked;

            pgSqlAuthCheck.Toggled += (_) =>
            {
                pgSqlUserText.Enabled = pgSqlAuthCheck.Checked;
                pgSqlPassText.Enabled = pgSqlAuthCheck.Checked;
            };

            Add(apiLabel, apiFqdnText, apiPortLabel, apiPortText);
            Add(mongoLabel, mongoFqdnText, mongoPortLabel, mongoPortText);
            Add(pgSqlLabel, pgSqlFqdnText, pgSqlPortLabel, pgSqlPortText, pgSqlDbLabel, pgSqlDbText);
            Add(pgSqlAuthCheck, pgSqlUserLabel, pgSqlUserText, pgSqlPassLabel, pgSqlPassText);

            var btnOk = new Button("OK", true);
            btnOk.Clicked += () =>
            {
                ClientApiFqdn = apiFqdnText.Text.ToString();
                ClientApiPort = apiPortText.Text.ToString();
                MongoDbFqdn = mongoFqdnText.Text.ToString();
                MongoDbPort = mongoPortText.Text.ToString();
                PgSqlDbFqdn = pgSqlFqdnText.Text.ToString();
                PgSqlDbPort = pgSqlPortText.Text.ToString();
                PgSqlDbName = pgSqlDbText.Text.ToString();

                if (pgSqlAuthCheck.Checked)
                {
                    PgSqlUser = pgSqlUserText.Text.ToString();
                    PgSqlPassword = pgSqlPassText.Text.ToString();
                }
                else
                {
                    PgSqlUser = string.Empty;
                    PgSqlPassword = string.Empty;
                }

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