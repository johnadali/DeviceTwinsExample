#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "parson.h"   // for JSON objects

// Indicate whether WiFi is ready
static bool hasWifi = false;

// Indicate whether IoT Hub is ready
static bool hasIoTHub = false;


static void InitWiFi()
{
  Screen.clean();
  Screen.print(2, "Connecting...");
  
  if (WiFi.begin() == WL_CONNECTED)
  {
    IPAddress ip = WiFi.localIP();
    Screen.print(1, ip.get_address());
    hasWifi = true;
    Screen.print(2, "Running... \r\n");
  }
  else
  {
    hasWifi = false;
    Screen.print(1, "No Wi-Fi\r\n ");
  }
}

static void DeviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
  char *temp = (char *)malloc(size + 1);

  if (temp == NULL)
  {
    return;
  }
  memcpy(temp, payLoad, size);
  temp[size] = '\0';

  Serial.printf("Desired Properties change from IoT Hub: %s\n", temp);

  free(temp);
}

void setup() {
  // put your setup code here, to run once:

  Screen.init();
  Screen.print(1, "Initializing...");

  Screen.print(2, "> Serial");
  Serial.begin(115200);

  // Initialize the WiFi module
  Screen.print(3, " > WiFi");
  hasWifi = false;
  InitWiFi();
  if (!hasWifi)
  {
    return;
  }

  // Initialize button
  Screen.print(3, " > Button");
  pinMode(USER_BUTTON_A, INPUT);
  pinMode(USER_BUTTON_B, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);

  Screen.print(3, " > IoT Hub");
  
  if (!DevKitMQTTClient_Init(true))   // Initializes MQTT client with Device Twin support
  {
    Screen.clean();
    Screen.print(2, "No IoT Hub");
    hasIoTHub = false;
    return;
  }
  hasIoTHub = true;

  DevKitMQTTClient_SetDeviceTwinCallback(DeviceTwinCallback);
}

void loop() {
  // put your main code here, to run repeatedly:


  if (hasWifi)
  {
    DevKitMQTTClient_Check();   // Checks if there is a message from IoT Hub

    if (digitalRead(USER_BUTTON_A) == LOW)
    {
      Screen.clean();
      Screen.print(2, "A Button pressed");
      Serial.printf("A Button pressed\n");

      long randNumber = random(300);
      char* state;
      JSON_Value* root_value = json_value_init_object();
      JSON_Object* root_object = json_value_get_object(root_value);

      // Serialize reported properties
      (void)json_object_dotset_string(root_object, "wifi.wifiIP", WiFi.localIP().get_address());
      (void)json_object_dotset_string(root_object, "wifi.wifiMask", WiFi.subnetMask().get_address());
      (void)json_object_dotset_number(root_object, "randomNumber", randNumber);

      state = json_serialize_to_string(root_value);

      json_value_free(root_value);

      Serial.printf("state: %s\n", state);

      DevKitMQTTClient_ReportState(state);    // Report state (reported properties) to IoT Hub
    }
    else if (digitalRead(USER_BUTTON_B) == LOW)
    {
      Screen.clean();
      Screen.print(2, "B Button pressed");
      Serial.printf("B Button pressed\n");
    }
  }

  delay(1000);  // Delay 1 second

}
