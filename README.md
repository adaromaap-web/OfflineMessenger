# OfflineMessenger

OfflineMessenger is an experimental private messenger designed for **direct, offline communication over Bluetooth**, without using the Internet.

The project currently includes Windows and Android clients. Messages are transferred directly between devices, encrypted during transmission, and stored locally as chat history.

## Features

*  Direct Bluetooth communication
*  Encrypted message transmission
*  Secure session establishment through key exchange
*  Local message history
*  Full conversation history stored on both devices
*  Message delivery acknowledgements
*  Windows client
*  Android client

### Core Components

#### ChatEngine

Handles the main chat logic:

* handshake;
* cryptographic key exchange;
* session key generation;
* message encryption and decryption;
* message sending and receiving;
* delivery acknowledgements.

#### BluetoothTransport

Responsible for transferring byte data directly between devices over Bluetooth.

#### CryptoService

Provides encryption and decryption of messages.

The current implementation uses **AES-GCM** for message encryption. A shared secret is established through key exchange and processed with **HKDF** to derive the session key.

#### MessageHistory

Responsible for storing the local conversation history in SQLite.

The Windows client uses Entity Framework Core, while the Android client has its own local database layer.

## Message Storage

Each stored message contains:

```text
Id
Text
IsMine
Timestamp
Status
```

The complete conversation is stored on **both devices**.

When sending a message:

1. The message is encrypted and sent through `ChatEngine`.
2. The message is added to the local chat UI.
3. The message is saved to the local SQLite database.

When receiving a message:

1. The Bluetooth packet is received.
2. The message is decrypted.
3. An acknowledgement (ACK) is sent back to the sender.
4. The message is added to the chat UI.
5. The message is saved to the local database.

This means that both participants maintain their own complete copy of the conversation.

> **Current limitation:** messages are currently stored in plaintext inside the local SQLite database. Local database encryption is planned as a future improvement.

## Windows

The Windows solution is located at:

```text
Windows/OfflineMessenger.slnx
```

Run from PowerShell:

```powershell
dotnet run --project .\Windows\OfflineMessenger.UI.Windows
```

Alternatively, open the solution in Visual Studio or JetBrains Rider and run the `OfflineMessenger.UI.Windows` project.

The Windows message database is stored at:

```text
%APPDATA%\OfflineMessenger\messages.db
```

## Android

The Android project is located in:

```text
Android/
```

Open the `Android` directory in Android Studio.

Build the project using:

```text
Build → Make Project
```

Then run the application on a connected Android device or emulator.

The Android client requires Bluetooth permissions, including:

```text
BLUETOOTH_CONNECT
BLUETOOTH_SCAN
```

## Testing

The current implementation can be tested using:

```text
Windows ↔ Android
```

After establishing a Bluetooth connection, the clients perform a cryptographic handshake.

The following should then be tested:

* Windows → Android message delivery
* Android → Windows message delivery
* delivery acknowledgements
* incoming and outgoing messages appearing in the UI
* message persistence in the local database
* complete conversation history after restarting either application

## Current Status

### Implemented

* [x] Bluetooth transport
* [x] Windows client
* [x] Android client
* [x] Handshake
* [x] Cryptographic key exchange
* [x] HKDF
* [x] AES-GCM encryption
* [x] Message sending
* [x] Message receiving
* [x] Delivery acknowledgements
* [x] SQLite message history
* [x] Conversation history restoration after restart
* [x] Full conversation history stored on both devices

### Planned

* [ ] Local database encryption
* [ ] More robust Bluetooth connection management
* [ ] Automatic reconnection
* [ ] Improved message delivery states
* [ ] Multiple chat sessions
* [ ] Peer identification
* [ ] More comprehensive cryptographic protocol testing
* [ ] Improved Android UI
* [ ] Application packaging and distribution

## Project Status

OfflineMessenger is currently an experimental project intended for learning and experimentation with:

* Bluetooth communication
* cryptographic protocols
* secure key exchange
* encrypted messaging
* local data storage
* Windows and Android application development

The current implementation should **not be considered a production-grade secure messenger**.
