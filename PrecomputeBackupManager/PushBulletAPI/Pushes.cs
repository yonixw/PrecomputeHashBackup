using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using PrivateData = PrecomputeBackupManager.Properties.Settings;

namespace PrecomputeBackupManager.PushBulletAPI
{
    class PushNoteObject
    {
        public bool active;
        public string iden;
        public double created;
        public string body;
        public string type;
    }

    class Pushes
    {
        static JavaScriptSerializer json = new JavaScriptSerializer();

        internal class CreatePushNoteJson {
            public string body;
            public string title;
            public string type;
        }

        internal class PushNoteListObject {
            public PushNoteObject[] pushes;
        }

        public static PushNoteObject[] getLastMessages(int count, double afterTime) {

            /*
            curl --header 'Access-Token: <your_access_token_here>' \
             --data-urlencode active="true" \
             --data-urlencode modified_after="1.4e+09" \
             --get \
             https://api.pushbullet.com/v2/pushes
            */

            /*
            {
              "pushes": [
                {
                  "active": true,
                  "body": "Space Elevator, Mars Hyperloop, Space Model S (Model Space?)",
                  "created": 1.412047948579029e+09,
                  "direction": "self",
                  "dismissed": false,
                  "iden": "ujpah72o0sjAoRtnM0jc",
                  "modified": 1.412047948579031e+09,
                  "receiver_email": "elon@teslamotors.com",
                  "receiver_email_normalized": "elon@teslamotors.com",
                  "receiver_iden": "ujpah72o0",
                  "sender_email": "elon@teslamotors.com",
                  "sender_email_normalized": "elon@teslamotors.com",
                  "sender_iden": "ujpah72o0",
                  "sender_name": "Elon Musk",
                  "title": "Space Travel Ideas",
                  "type": "note"
                }
              ]
            }
            */

            WebClient wb = new WebClient();
            wb.Headers.Add("Access-Token", PrivateData.Default.PBAuthCode);
            string response = wb.DownloadString(
                "https://api.pushbullet.com/v2/pushes"
                + "?active=true"
                + "&modified_after=" + afterTime.ToString()
            );


            PushNoteListObject list = json.Deserialize<PushNoteListObject>(response);

            return list.pushes;
        }


        public static PushNoteObject createPushNote(string title , string body) {
            // SO? 13642873/1997873

            /*
            curl --header 'Access-Token: <your_access_token_here>' \
                 --header 'Content-Type: application/json' \
                 --data-binary '{"body":"Space Elevator, Mars Hyperloop, Space Model S (Model Space?)","title":"Space Travel Ideas","type":"note"}' \
                 --request POST \
                 https://api.pushbullet.com/v2/pushes

            */

            /*
            {
              "active": true,
              "body": "Space Elevator, Mars Hyperloop, Space Model S (Model Space?)",
              "created": 1.412047948579029e+09,
              "direction": "self",
              "dismissed": false,
              "iden": "ujpah72o0sjAoRtnM0jc",
              "modified": 1.412047948579031e+09,
              "receiver_email": "elon@teslamotors.com",
              "receiver_email_normalized": "elon@teslamotors.com",
              "receiver_iden": "ujpah72o0",
              "sender_email": "elon@teslamotors.com",
              "sender_email_normalized": "elon@teslamotors.com",
              "sender_iden": "ujpah72o0",
              "sender_name": "Elon Musk",
              "title": "Space Travel Ideas",
              "type": "note"
            }
            */

            CreatePushNoteJson pushJson = new CreatePushNoteJson();
            pushJson.body = body;
            pushJson.title = title;
            pushJson.type = "note";

            WebClient wb = new WebClient();
            wb.Headers.Add("Access-Token", PrivateData.Default.PBAuthCode);
            wb.Headers.Add("Content-Type", "application/json");
            string response = wb.UploadString("https://api.pushbullet.com/v2/pushes", "POST", json.Serialize(pushJson));

            PushNoteObject respObject = json.Deserialize<PushNoteObject>(response);
            return respObject;
        }
    }
}
