using Terminal.Gui;

namespace InventarViewerApp.UI
{
    public class SettingsDialog : Dialog
    {
        public string ClientApiFqdn { get; private set; }
        public string ClientApiPort { get; private set; }
        public string MongoDbFqdn { get; private set; }
        public string MongoDbPort { get; private set; }
        public string MongoDbUser { get; private set; }
        public string MongoDbPassword { get; private set; }
        public string PgSqlDbFqdn { get; private set; }
        public string PgSqlDbPort { get; private set; }
        public string PgSqlDbName { get; private set; }
        public string PgSqlUser { get; private set; }
        public string PgSqlPassword { get; private set; }
        
        public bool Canceled { get; private set; } = true;

        public SettingsDialog(
            string currentApiFqdn = "localhost", string currentApiPort = "80",
            string currentMongoFqdn = "localhost", string currentMongoPort = "27017",
            string currentMongoUser = "", string currentMongoPassword = "",
            string currentPgSqlFqdn = "localhost", string currentPgSqlPort = "5432",
            string currentPgSqlDbName = "postgres",
            string currentPgSqlUser = "", string currentPgSqlPassword = "") 
            : base("Einstellungen", 60, 24) // Höhe vergrößert für neue Felder
        {
            // --- Client API Group ---
            var apiFrame = new FrameView("Client API")
            {
                X = 1,
                Y = 0,
                Width = Dim.Fill(1),
                Height = 4
            };

            var apiLabel = new Label("FQDN:") { X = 1, Y = 0 };
            var apiFqdnText = new TextField(currentApiFqdn) { X = 12, Y = 0, Width = Dim.Fill(1) };
            var apiPortLabel = new Label("Port:") { X = 1, Y = 1 };
            var apiPortText = new TextField(currentApiPort) { X = 12, Y = 1, Width = 10 };
            
            apiFrame.Add(apiLabel, apiFqdnText, apiPortLabel, apiPortText);

            // --- MongoDB Group ---
            var mongoFrame = new FrameView("MongoDB")
            {
                X = 1,
                Y = Pos.Bottom(apiFrame),
                Width = Dim.Fill(1),
                Height = 8 // Höhe angepasst
            };

            var mongoLabel = new Label("FQDN:") { X = 1, Y = 0 };
            var mongoFqdnText = new TextField(currentMongoFqdn) { X = 12, Y = 0, Width = Dim.Fill(1) };
            var mongoPortLabel = new Label("Port:") { X = 1, Y = 1 };
            var mongoPortText = new TextField(currentMongoPort) { X = 12, Y = 1, Width = 10 };
            
            var mongoAuthCheck = new CheckBox("Authentifizierung", !string.IsNullOrEmpty(currentMongoUser)) { X = 1, Y = 3 };
            
            var mongoUserLabel = new Label("User:") { X = 1, Y = 4 };
            var mongoUserText = new TextField(currentMongoUser) { X = 12, Y = 4, Width = Dim.Fill(1) };
            
            var mongoPassLabel = new Label("Passwort:") { X = 1, Y = 5 };
            var mongoPassText = new TextField(currentMongoPassword) { X = 12, Y = 5, Width = Dim.Fill(1), Secret = true };

            // Initialer Status basierend auf Checkbox
            mongoUserText.Enabled = mongoAuthCheck.Checked;
            mongoPassText.Enabled = mongoAuthCheck.Checked;

            mongoAuthCheck.Toggled += (_) =>
            {
                mongoUserText.Enabled = mongoAuthCheck.Checked;
                mongoPassText.Enabled = mongoAuthCheck.Checked;
            };

            mongoFrame.Add(mongoLabel, mongoFqdnText, mongoPortLabel, mongoPortText, 
                           mongoAuthCheck, mongoUserLabel, mongoUserText, mongoPassLabel, mongoPassText);

            // --- PgSQL Group ---
            var pgSqlFrame = new FrameView("PostgreSQL")
            {
                X = 1,
                Y = Pos.Bottom(mongoFrame), // Positioniert unter MongoFrame
                Width = Dim.Fill(1),
                Height = 9
            };

            var pgSqlLabel = new Label("FQDN:") { X = 1, Y = 0 };
            var pgSqlFqdnText = new TextField(currentPgSqlFqdn) { X = 12, Y = 0, Width = Dim.Fill(1) };
            var pgSqlPortLabel = new Label("Port:") { X = 1, Y = 1 };
            var pgSqlPortText = new TextField(currentPgSqlPort) { X = 12, Y = 1, Width = 10 };
            var pgSqlDbLabel = new Label("DB Name:") { X = 1, Y = 2 }; 
            var pgSqlDbText = new TextField(currentPgSqlDbName) { X = 12, Y = 2, Width = Dim.Fill(1) }; 

            var pgSqlAuthCheck = new CheckBox("Authentifizierung", !string.IsNullOrEmpty(currentPgSqlUser)) { X = 1, Y = 4 };
            
            var pgSqlUserLabel = new Label("User:") { X = 1, Y = 5 };
            var pgSqlUserText = new TextField(currentPgSqlUser) { X = 12, Y = 5, Width = Dim.Fill(1) };
            
            var pgSqlPassLabel = new Label("Passwort:") { X = 1, Y = 6 };
            var pgSqlPassText = new TextField(currentPgSqlPassword) { X = 12, Y = 6, Width = Dim.Fill(1), Secret = true };

            // Initialer Status basierend auf Checkbox
            pgSqlUserText.Enabled = pgSqlAuthCheck.Checked;
            pgSqlPassText.Enabled = pgSqlAuthCheck.Checked;

            pgSqlAuthCheck.Toggled += (_) =>
            {
                pgSqlUserText.Enabled = pgSqlAuthCheck.Checked;
                pgSqlPassText.Enabled = pgSqlAuthCheck.Checked;
            };

            pgSqlFrame.Add(pgSqlLabel, pgSqlFqdnText, pgSqlPortLabel, pgSqlPortText, 
                           pgSqlDbLabel, pgSqlDbText, pgSqlAuthCheck, 
                           pgSqlUserLabel, pgSqlUserText, pgSqlPassLabel, pgSqlPassText);

            Add(apiFrame, mongoFrame, pgSqlFrame);

            var btnOk = new Button("OK", true);
            btnOk.Clicked += () =>
            {
                ClientApiFqdn = apiFqdnText.Text.ToString();
                ClientApiPort = apiPortText.Text.ToString();
                MongoDbFqdn = mongoFqdnText.Text.ToString();
                MongoDbPort = mongoPortText.Text.ToString();
                
                if (mongoAuthCheck.Checked)
                {
                    MongoDbUser = mongoUserText.Text.ToString();
                    MongoDbPassword = mongoPassText.Text.ToString();
                }
                else
                {
                    MongoDbUser = string.Empty;
                    MongoDbPassword = string.Empty;
                }

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