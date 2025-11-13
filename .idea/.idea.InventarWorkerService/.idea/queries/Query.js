/**
 * Select an Inventory database
 */
db.getCollectionNames();

db.getCollection('8').find();
db.getCollection('8').find({ "InstalledSoftware.Name":  "Microsoft Edge" });
// Unpack InstalledSoftware-Array for better readability
db.getCollection('8').aggregate([
    { $unwind: "$InstalledSoftware" },
    { $project: { "InstalledSoftware": 1 } }
]);

/**
 * Alternative: Regex-based search for versions
 */
db.getCollection('8').aggregate([
    { $unwind: "$InstalledSoftware" },
    {
        $match: {
            "InstalledSoftware.Version": {
                $regex: "^([1-9]|[1-9]\\d+)"
            }
        }
    },
    {
        $project: {
            "SoftwareName": "$InstalledSoftware.Name",
            "Version": "$InstalledSoftware.Version",
            "Publisher": "$InstalledSoftware.Publisher"
        }
    },
    { $sort: { "SoftwareName": -1, "Version": 1 } }
]);
db.getCollection("8").aggregate(
    {$unwind: "$InstalledSoftware"},
    {$project: {"SoftwareName": "$InstalledSoftware.Name", "Version": "$InstalledSoftware.Version", _id: 0}},
    {$sort: {"Version": -1, "SoftwareName": -1} }
);

db.getCollection("8").aggregate([
    { $unwind: "$RunningProcesses" },
    {
        $project: {
            "ProcessName": "$RunningProcesses.ProcessName",
            "ProcessId": "$RunningProcesses.ProcessId",
            "StartTimeOnly": {
                $dateToString: {
                    format: "%H:%M:%S",
                    date: "$RunningProcesses.StartTime"
                }
            },
            "StartDateOnly": {
                $dateToString: {
                    format: "%d.%m.%Y",
                    date: "$RunningProcesses.StartTime"
                }
            },
            _id: 0
        }
    },
    { $sort: { "StartTimeOnly": -1 } }
]);