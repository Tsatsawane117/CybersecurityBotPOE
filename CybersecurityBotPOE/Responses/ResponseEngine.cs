using CybersecurityBotPOE.Memory;

namespace CybersecurityBotPOE.Responses
{
    /// <summary>
    /// Core response engine. Handles keyword recognition, random response selection,
    /// follow-up expansion, and memory-aware personalisation.
    /// </summary>
    public class ResponseEngine
    {
        private readonly Random _random = new();
        private string? _lastDetectedTopic;

        // ─── Response Model ───────────────────────────────────────────────────────

        /// <summary>Represents one topic entry with multiple response variants.</summary>
        private class TopicEntry
        {
            public string TopicName { get; init; } = string.Empty;
            public string[] Keywords { get; init; } = Array.Empty<string>();
            /// <summary>Multiple variants — one is picked at random each time.</summary>
            public string[] Responses { get; init; } = Array.Empty<string>();
            /// <summary>Extra follow-up detail shown when the user says "tell me more".</summary>
            public string FollowUp { get; init; } = string.Empty;
        }

        // ─── Topic Catalogue ──────────────────────────────────────────────────────

        private readonly List<TopicEntry> _topics;

        public ResponseEngine()
        {
            _topics = BuildTopics();
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>Returns the topic name matched in the last GetResponse call.</summary>
        public string? GetLastDetectedTopic() => _lastDetectedTopic;

        /// <summary>
        /// Finds the best response for the user's input, personalised with their name
        /// and optionally enriched with a memory recall hint.
        /// </summary>
        public string GetResponse(string input, string userName, UserMemory memory)
        {
            memory.IncrementMessageCount();

            if (string.IsNullOrWhiteSpace(input))
                return $"I didn't quite catch that, {userName}. Could you rephrase?";

            string lower = input.ToLower();

            foreach (var topic in _topics)
            {
                if (topic.Keywords.Any(k => lower.Contains(k)))
                {
                    _lastDetectedTopic = topic.TopicName;

                    // Pick a random response variant
                    string response = topic.Responses[_random.Next(topic.Responses.Length)];
                    response = response.Replace("{name}", userName);

                    // Optionally append a memory recall note
                    string recall = memory.GetMemoryRecall();
                    if (!string.IsNullOrEmpty(recall) &&
                        memory.FavouriteTopic != topic.TopicName)
                    {
                        response += $"\n\n💡 {recall}";
                    }

                    return response;
                }
            }

            // Fallback
            _lastDetectedTopic = null;
            return $"I'm not sure I understand that, {userName}. " +
                   "You can ask me about passwords, phishing, malware, 2FA, encryption, " +
                   "VPNs, data breaches, and more. Type 'help' for a full list!";
        }

        /// <summary>Returns the follow-up expansion for the given topic name.</summary>
        public string GetFollowUp(string topicName, string userName)
        {
            var topic = _topics.FirstOrDefault(t =>
                t.TopicName.Equals(topicName, StringComparison.OrdinalIgnoreCase));

            if (topic == null)
                return $"I don't have extra details on '{topicName}' right now, {userName}. Try asking about a specific aspect!";

            return topic.FollowUp.Replace("{name}", userName);
        }

        // ─── Topic Definitions ────────────────────────────────────────────────────

        private static List<TopicEntry> BuildTopics() => new()
        {
            // ── Help / Menu ───────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "help",
                Keywords  = new[] { "help", "menu", "topics", "what can you do", "what can i ask" },
                Responses = new[]
                {
                    "Here are the topics I can help you with, {name}:\n\n" +
                    "  🔑  Passwords & password managers\n" +
                    "  🎣  Phishing, smishing & vishing\n" +
                    "  🦠  Malware, viruses & ransomware\n" +
                    "  🔐  Two-factor authentication (2FA)\n" +
                    "  🔒  Encryption\n" +
                    "  📶  Public Wi-Fi & VPNs\n" +
                    "  🧱  Firewalls\n" +
                    "  🕵️  Social engineering\n" +
                    "  🌐  Safe browsing & online shopping\n" +
                    "  🧬  Data breaches & identity theft\n" +
                    "  ☁️  Cloud security\n" +
                    "  🔄  Software updates & backups\n" +
                    "  👶  Children's online safety\n\n" +
                    "Just type any of these topics and I'll help!"
                },
                FollowUp = "Feel free to ask about any specific topic, {name}, " +
                           "and I'll give you a detailed breakdown!"
            },

            // ── Password Safety ───────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "passwords",
                Keywords  = new[] { "password", "passwords", "passphrase", "credentials" },
                Responses = new[]
                {
                    "Strong passwords are your first line of defence, {name}!\n\n" +
                    "  • Use at least 12 characters.\n" +
                    "  • Mix uppercase, lowercase, numbers, and symbols.\n" +
                    "  • Never reuse passwords across sites.\n" +
                    "  • A passphrase like 'PurpleCat$RunsFast9' is strong AND memorable.\n" +
                    "  • Consider a password manager — it remembers them all for you!",

                    "Great question about passwords, {name}!\n\n" +
                    "  • The longer the password, the harder it is to crack.\n" +
                    "  • Avoid personal details like birthdays or pet names.\n" +
                    "  • Enable 2FA alongside strong passwords for double protection.\n" +
                    "  • Change passwords immediately if you suspect a breach.",

                    "Password safety is crucial, {name}! Here's what you need to know:\n\n" +
                    "  • 'Password123' is one of the most commonly hacked passwords — avoid it!\n" +
                    "  • Use a different password for every account.\n" +
                    "  • A password manager like Bitwarden makes this easy and free.\n" +
                    "  • Never share your passwords, even with people you trust."
                },
                FollowUp =
                    "Going deeper on passwords, {name}:\n\n" +
                    "  • Brute-force attacks try millions of combinations per second — length is key.\n" +
                    "  • Dictionary attacks use common words, so avoid them.\n" +
                    "  • The NIST guidelines now recommend long passphrases over complex short passwords.\n" +
                    "  • Enable 'Have I Been Pwned' alerts to know if your password leaks online."
            },

            // ── Password Manager ──────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "password manager",
                Keywords  = new[] { "password manager", "bitwarden", "lastpass", "1password", "keepass" },
                Responses = new[]
                {
                    "Password managers are game-changers, {name}!\n\n" +
                    "  • They generate strong, unique passwords for every site.\n" +
                    "  • You only need to remember ONE master password.\n" +
                    "  • Bitwarden is free, open-source, and highly trusted.\n" +
                    "  • Always protect the manager itself with a strong master password + 2FA.",

                    "Using a password manager is one of the best things you can do, {name}!\n\n" +
                    "  • Top options: Bitwarden (free), 1Password, KeePass.\n" +
                    "  • They autofill passwords so you never mistype them.\n" +
                    "  • They also alert you to duplicate or weak passwords.\n" +
                    "  • Your passwords are encrypted — even the company can't see them."
                },
                FollowUp =
                    "More on password managers, {name}:\n\n" +
                    "  • Use the browser extension for seamless autofill.\n" +
                    "  • Enable emergency access for trusted contacts.\n" +
                    "  • Export and back up your vault regularly in a secure location.\n" +
                    "  • If your master password is lost, most managers cannot recover it — choose carefully!"
            },

            // ── Phishing ──────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "phishing",
                Keywords  = new[] { "phishing", "phish", "scam", "scams", "fake email", "suspicious email", "spam" },
                Responses = new[]
                {
                    "Phishing is one of the most common attacks, {name}! Watch for:\n\n" +
                    "  • Urgent language: 'Act NOW or your account is closed!'\n" +
                    "  • Sender email doesn't match the organisation (e.g. support@amaz0n.net).\n" +
                    "  • Hover over links before clicking — do they go where they say?\n" +
                    "  • Unexpected attachments, especially .exe or .zip files.\n" +
                    "When in doubt — don't click! Report it.",

                    "Phishing tricks you into giving up information, {name}. Stay safe:\n\n" +
                    "  • Legitimate banks NEVER ask for your PIN or password by email.\n" +
                    "  • Look for HTTPS and check the full domain name carefully.\n" +
                    "  • If it creates panic or urgency, it's almost certainly a scam.\n" +
                    "  • Use email filtering tools and report phishing attempts.",

                    "Great awareness asking about phishing, {name}! Here's a tip:\n\n" +
                    "  • Spear phishing targets YOU specifically — using your name and details.\n" +
                    "  • Check for subtle misspellings: 'Micros0ft', 'Paypa1', 'Arnazon'.\n" +
                    "  • Phone calls can also be phishing — this is called 'vishing'.\n" +
                    "  • SMS phishing is called 'smishing' — never click unexpected text links."
                },
                FollowUp =
                    "Digging deeper into phishing, {name}:\n\n" +
                    "  • 'Whaling' targets senior executives with highly convincing emails.\n" +
                    "  • Phishing kits are sold on the dark web — attacks are becoming automated.\n" +
                    "  • Use DMARC/DKIM email authentication to reduce phishing from your own domain.\n" +
                    "  • Report phishing to your national authority (e.g. reportphishing.antiphishing.org)."
            },

            // ── Smishing / Vishing ────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "smishing and vishing",
                Keywords  = new[] { "smishing", "vishing", "sms scam", "text scam", "phone scam", "call scam" },
                Responses = new[]
                {
                    "Smishing (SMS) and vishing (voice) are growing threats, {name}!\n\n" +
                    "  • Never click links in unexpected text messages.\n" +
                    "  • Real companies never ask for passwords or PINs over phone.\n" +
                    "  • If your 'bank' calls you, hang up and call them back on their official number.\n" +
                    "  • Caller ID can be spoofed — don't trust the number shown."
                },
                FollowUp =
                    "More on SMS and phone scams, {name}:\n\n" +
                    "  • Scammers often impersonate SARS, your bank, or SAPS.\n" +
                    "  • Use call-blocking apps to filter known scam numbers.\n" +
                    "  • Register on the Do Not Contact list if available in your country.\n" +
                    "  • Educate family members — elderly people are frequently targeted."
            },

            // ── Malware ───────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "malware",
                Keywords  = new[] { "malware", "virus", "trojan", "spyware", "antivirus", "worm", "adware" },
                Responses = new[]
                {
                    "Malware is malicious software designed to harm your system, {name}!\n\n" +
                    "  • Viruses spread by attaching to files; worms spread over networks.\n" +
                    "  • Trojans disguise themselves as legitimate software.\n" +
                    "  • Spyware silently monitors your activity.\n" +
                    "  • Keep antivirus updated and never open unexpected attachments.",

                    "Protecting against malware is essential, {name}!\n\n" +
                    "  • Use reputable antivirus software (Windows Defender is decent built-in).\n" +
                    "  • Scan USB drives before opening files from them.\n" +
                    "  • Keep your OS updated — patches close the doors malware uses.\n" +
                    "  • Avoid pirated software — it's a common malware delivery method."
                },
                FollowUp =
                    "More on malware, {name}:\n\n" +
                    "  • Rootkits hide deep in the OS making them very hard to detect.\n" +
                    "  • Keyloggers record every keystroke — including your passwords.\n" +
                    "  • Fileless malware lives in memory only — leaving no file traces.\n" +
                    "  • If infected, disconnect from the internet immediately and seek help."
            },

            // ── Ransomware ────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "ransomware",
                Keywords  = new[] { "ransomware", "ransom", "files locked", "encrypted files", "pay hackers" },
                Responses = new[]
                {
                    "Ransomware encrypts your files and demands payment, {name}!\n\n" +
                    "  • Prevention is everything — keep backups and software updated.\n" +
                    "  • Do NOT pay the ransom — it doesn't guarantee your files return.\n" +
                    "  • Disconnect the infected machine immediately from the network.\n" +
                    "  • Report the attack to your country's cybercrime unit.",

                    "Ransomware attacks are devastating, {name}. Here's how to stay protected:\n\n" +
                    "  • Keep offline backups — ransomware can't encrypt what it can't reach.\n" +
                    "  • Never open email attachments from unknown senders.\n" +
                    "  • Disable macros in Office documents unless you specifically need them.\n" +
                    "  • Use endpoint detection tools if you run a business."
                },
                FollowUp =
                    "Going deeper on ransomware, {name}:\n\n" +
                    "  • WannaCry (2017) infected 230,000+ computers in 150 countries.\n" +
                    "  • Ransomware-as-a-Service (RaaS) lets criminals 'rent' attack tools.\n" +
                    "  • The average ransom demand in 2024 exceeded $1 million for businesses.\n" +
                    "  • Always follow the 3-2-1 backup rule to recover without paying."
            },

            // ── 2FA ───────────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "two-factor authentication",
                Keywords  = new[] { "2fa", "two factor", "two-factor", "mfa", "multi factor", "authenticator" },
                Responses = new[]
                {
                    "2FA adds a second lock on your account, {name}!\n\n" +
                    "  • Even if someone steals your password, they need your 2FA code too.\n" +
                    "  • Use an authenticator app (Google Authenticator, Authy) over SMS.\n" +
                    "  • SMS codes can be intercepted via SIM-swapping attacks.\n" +
                    "  • Hardware keys (YubiKey) offer the strongest protection.",

                    "Two-factor authentication is a must-have, {name}!\n\n" +
                    "  • Enable 2FA on your email, banking, and social media first.\n" +
                    "  • Backup codes are given when you set up 2FA — store them safely offline.\n" +
                    "  • If you lose access to your 2FA app, use the backup codes to recover.\n" +
                    "  • Most major sites now support 2FA — check settings > security."
                },
                FollowUp =
                    "More on 2FA, {name}:\n\n" +
                    "  • TOTP (Time-based One-Time Passwords) are the most common authenticator type.\n" +
                    "  • Passkeys are the next evolution — biometric logins replacing passwords entirely.\n" +
                    "  • FIDO2/WebAuthn standards allow phishing-resistant authentication.\n" +
                    "  • Even basic SMS 2FA is far better than no 2FA at all."
            },

            // ── Encryption ────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "encryption",
                Keywords  = new[] { "encryption", "encrypt", "encrypted", "decrypt", "end-to-end", "e2ee" },
                Responses = new[]
                {
                    "Encryption keeps your data unreadable to anyone without the key, {name}!\n\n" +
                    "  • HTTPS encrypts data between you and websites — always check for it.\n" +
                    "  • End-to-end encryption (E2EE) means only you and the recipient can read messages.\n" +
                    "  • Enable full-disk encryption: BitLocker (Windows) or FileVault (Mac).\n" +
                    "  • Encrypted data stolen in a breach is useless without the key.",

                    "Great question on encryption, {name}!\n\n" +
                    "  • AES-256 is the gold standard encryption algorithm — used by governments.\n" +
                    "  • Signal and WhatsApp use E2EE for messages.\n" +
                    "  • Your phone's storage should be encrypted — most modern phones do this by default.\n" +
                    "  • VPNs encrypt your internet traffic to protect you on public Wi-Fi."
                },
                FollowUp =
                    "Deeper on encryption, {name}:\n\n" +
                    "  • Symmetric encryption uses one key; asymmetric uses a public/private key pair.\n" +
                    "  • TLS (Transport Layer Security) is what HTTPS uses under the hood.\n" +
                    "  • Quantum computing may threaten current encryption — post-quantum algorithms are being developed.\n" +
                    "  • Certificate pinning prevents man-in-the-middle attacks on apps."
            },

            // ── Public Wi-Fi / VPN ────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "public Wi-Fi and VPNs",
                Keywords  = new[] { "public wifi", "public wi-fi", "free wifi", "hotspot", "vpn", "open network" },
                Responses = new[]
                {
                    "Public Wi-Fi is risky without protection, {name}!\n\n" +
                    "  • Anyone on the same network can potentially spy on unencrypted traffic.\n" +
                    "  • Use a VPN to encrypt all your traffic on public networks.\n" +
                    "  • Avoid logging into banking or sensitive accounts on public Wi-Fi.\n" +
                    "  • Turn off auto-connect to open Wi-Fi networks on your phone.",

                    "Stay safe on public networks, {name}!\n\n" +
                    "  • Evil twin attacks create fake hotspots named 'Coffee Shop Free WiFi'.\n" +
                    "  • A reputable VPN (ProtonVPN, Mullvad) encrypts everything you send.\n" +
                    "  • Free VPNs often sell your data — avoid them.\n" +
                    "  • Using your phone as a personal hotspot is safer than public Wi-Fi."
                },
                FollowUp =
                    "More on VPNs and Wi-Fi, {name}:\n\n" +
                    "  • A VPN routes traffic through an encrypted tunnel to a remote server.\n" +
                    "  • It hides your IP address and location from websites.\n" +
                    "  • VPNs don't make you anonymous — your VPN provider can still see traffic.\n" +
                    "  • For maximum privacy, combine a VPN with Tor — though this is slower."
            },

            // ── Firewall ──────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "firewalls",
                Keywords  = new[] { "firewall", "network security", "network protection", "intrusion" },
                Responses = new[]
                {
                    "Firewalls are your network's security guards, {name}!\n\n" +
                    "  • They monitor and filter incoming/outgoing network traffic.\n" +
                    "  • Windows and macOS have built-in firewalls — keep them enabled!\n" +
                    "  • Your home router also acts as a basic hardware firewall.\n" +
                    "  • Never disable your firewall unless you know exactly what you're doing."
                },
                FollowUp =
                    "More on firewalls, {name}:\n\n" +
                    "  • Next-gen firewalls (NGFW) do deep packet inspection — detecting hidden threats.\n" +
                    "  • Web Application Firewalls (WAF) protect websites from SQL injection and XSS attacks.\n" +
                    "  • Configure your firewall to block all inbound connections by default.\n" +
                    "  • Regularly review which apps have firewall exceptions on your device."
            },

            // ── Social Engineering ────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "social engineering",
                Keywords  = new[] { "social engineering", "manipulation", "pretexting", "impersonation", "baiting" },
                Responses = new[]
                {
                    "Social engineering exploits human psychology, not technology, {name}!\n\n" +
                    "  • Pretexting: attacker creates a fake scenario to earn your trust.\n" +
                    "  • Baiting: leaving infected USB drives where victims will find them.\n" +
                    "  • Tailgating: following someone into a secure area without authorisation.\n" +
                    "  • Always verify identities before sharing ANY sensitive information.",

                    "Humans are often the weakest link, {name} — and attackers know it!\n\n" +
                    "  • Be sceptical of unexpected requests, even from 'colleagues'.\n" +
                    "  • Verify out-of-band — if IT calls asking for your password, call them back.\n" +
                    "  • Authority and urgency are social engineers' favourite tools.\n" +
                    "  • Security awareness training dramatically reduces social engineering success."
                },
                FollowUp =
                    "Going deeper on social engineering, {name}:\n\n" +
                    "  • The 2020 Twitter hack used social engineering on Twitter employees.\n" +
                    "  • Kevin Mitnick, one of history's most famous hackers, relied almost entirely on social engineering.\n" +
                    "  • 'Quid pro quo' attacks offer something in exchange for information or access.\n" +
                    "  • Physical security matters too — shred sensitive documents, don't let strangers tailgate."
            },

            // ── Safe Browsing ─────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "safe browsing",
                Keywords  = new[] { "browsing", "safe browsing", "https", "website", "browser", "secure site" },
                Responses = new[]
                {
                    "Safe browsing habits make a big difference, {name}!\n\n" +
                    "  • Always check for HTTPS (padlock icon) before entering sensitive data.\n" +
                    "  • Keep your browser updated — updates patch security vulnerabilities.\n" +
                    "  • Use extensions like uBlock Origin to block malicious ads.\n" +
                    "  • Be wary of pop-ups claiming your device is infected.",

                    "Browse smarter, not just safer, {name}!\n\n" +
                    "  • Brave and Firefox offer better privacy than Chrome by default.\n" +
                    "  • Use DuckDuckGo to avoid Google's extensive tracking.\n" +
                    "  • Regularly clear cookies and browsing history.\n" +
                    "  • Check site reputation with tools like Google Safe Browsing or VirusTotal."
                },
                FollowUp =
                    "More on safe browsing, {name}:\n\n" +
                    "  • DNS-over-HTTPS (DoH) prevents ISPs from seeing which sites you visit.\n" +
                    "  • Browser fingerprinting tracks you even without cookies — use anti-fingerprinting tools.\n" +
                    "  • Private/Incognito mode hides history locally but NOT from your ISP or employer.\n" +
                    "  • The EFF's 'Cover Your Tracks' tool shows how unique your browser fingerprint is."
            },

            // ── Data Backup ───────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "data backups",
                Keywords  = new[] { "backup", "back up", "data loss", "3-2-1", "recovery" },
                Responses = new[]
                {
                    "Regular backups are your safety net, {name}! Follow the 3-2-1 rule:\n\n" +
                    "  • 3 copies of your data.\n" +
                    "  • 2 stored on different media types (external drive + cloud).\n" +
                    "  • 1 kept offsite or offline.\n" +
                    "Test your backups regularly — an untested backup might not restore correctly!",

                    "Don't wait for disaster to think about backups, {name}!\n\n" +
                    "  • Cloud services like Google Drive, OneDrive, or Backblaze are great options.\n" +
                    "  • Schedule automatic backups so you never forget.\n" +
                    "  • Keep at least one offline backup — ransomware can't reach what's disconnected.\n" +
                    "  • Back up your phone too — Google Photos or iCloud work well for this."
                },
                FollowUp =
                    "More on backups, {name}:\n\n" +
                    "  • Incremental backups save only changes since the last backup — faster and smaller.\n" +
                    "  • Versioned backups let you restore to a previous point before ransomware struck.\n" +
                    "  • Immutable backups cannot be altered or deleted — the gold standard against ransomware.\n" +
                    "  • RPO (Recovery Point Objective) and RTO (Recovery Time Objective) define backup strategy in businesses."
            },

            // ── Identity Theft ────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "identity theft",
                Keywords  = new[] { "identity theft", "identity fraud", "stolen identity", "personal info", "personal information" },
                Responses = new[]
                {
                    "Identity theft is serious, {name}! Here's how to protect yourself:\n\n" +
                    "  • Never share your ID number, banking details, or passwords via email or phone.\n" +
                    "  • Shred physical documents with personal information.\n" +
                    "  • Monitor your bank statements monthly for unauthorised transactions.\n" +
                    "  • Use credit monitoring services to be alerted to suspicious activity.",

                    "Guard your identity carefully, {name}!\n\n" +
                    "  • Criminals only need your ID number and basic details to open accounts in your name.\n" +
                    "  • Check your credit report annually for accounts you didn't open.\n" +
                    "  • Be careful what you post on social media — your birthdate and city are valuable to fraudsters.\n" +
                    "  • Report identity theft to SAPS and your bank immediately if you're victimised."
                },
                FollowUp =
                    "More on identity theft, {name}:\n\n" +
                    "  • Synthetic identity fraud combines real and fake information to create new identities.\n" +
                    "  • Dark web monitoring services alert you if your info is found for sale.\n" +
                    "  • A credit freeze prevents new accounts being opened in your name — very effective.\n" +
                    "  • Recovery from identity theft can take months to years — prevention is everything."
            },

            // ── Data Breaches ─────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "data breaches",
                Keywords  = new[] { "breach", "data breach", "leaked", "have i been pwned", "dark web", "darkweb" },
                Responses = new[]
                {
                    "Your data may have been exposed in a breach, {name}!\n\n" +
                    "  • Visit haveibeenpwned.com to check if your email appeared in a breach.\n" +
                    "  • If compromised, change that password everywhere you used it.\n" +
                    "  • Enable breach alerts to be notified of future incidents.\n" +
                    "  • Using unique passwords per site means one breach doesn't cascade.",

                    "Data breaches expose millions of people's information, {name}!\n\n" +
                    "  • Breached data is sold on the dark web — often within hours of the attack.\n" +
                    "  • Common breached data includes emails, hashed passwords, phone numbers, and addresses.\n" +
                    "  • Hashed passwords can still be cracked if they're weak or common.\n" +
                    "  • Enable 2FA so that a leaked password alone can't compromise your account."
                },
                FollowUp =
                    "More on data breaches, {name}:\n\n" +
                    "  • The 2013 Yahoo breach exposed 3 billion accounts — the largest ever.\n" +
                    "  • GDPR in Europe requires companies to report breaches within 72 hours.\n" +
                    "  • POPIA in South Africa similarly requires breach notification.\n" +
                    "  • Credential stuffing uses leaked username/password combos to attack other sites automatically."
            },

            // ── Privacy ───────────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "privacy",
                Keywords  = new[] { "privacy", "personal data", "data privacy", "tracking", "surveillance" },
                Responses = new[]
                {
                    "Online privacy is increasingly important, {name}!\n\n" +
                    "  • Review privacy settings on all your social media accounts regularly.\n" +
                    "  • Limit what apps can access — location, contacts, microphone, camera.\n" +
                    "  • Use a privacy-focused browser like Firefox or Brave.\n" +
                    "  • DuckDuckGo doesn't track your searches — a great Google alternative.",

                    "Protecting your privacy starts with awareness, {name}!\n\n" +
                    "  • Advertisers build detailed profiles based on your browsing and app usage.\n" +
                    "  • Read app permissions carefully before installing anything.\n" +
                    "  • Opt out of data brokers who sell your personal information.\n" +
                    "  • Consider using a privacy-focused email like ProtonMail."
                },
                FollowUp =
                    "Deeper on privacy, {name}:\n\n" +
                    "  • Browser fingerprinting identifies you without cookies — a growing threat.\n" +
                    "  • The Tor network anonymises your traffic by routing it through multiple relays.\n" +
                    "  • Data minimisation — only share the minimum required — is a good default habit.\n" +
                    "  • POPIA gives South Africans the right to know what data organisations hold on them."
            },

            // ── Cloud Security ────────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "cloud security",
                Keywords  = new[] { "cloud", "cloud storage", "google drive", "onedrive", "dropbox", "icloud" },
                Responses = new[]
                {
                    "Cloud storage is convenient but needs careful security, {name}!\n\n" +
                    "  • Enable 2FA on every cloud account — email, Drive, OneDrive, Dropbox.\n" +
                    "  • Don't store unencrypted sensitive files (ID scans, passwords) in the cloud.\n" +
                    "  • Review which third-party apps have access to your cloud storage.\n" +
                    "  • Use strong, unique passwords for each cloud service."
                },
                FollowUp =
                    "More on cloud security, {name}:\n\n" +
                    "  • Misconfigured cloud storage (public S3 buckets) is responsible for many major breaches.\n" +
                    "  • Client-side encryption tools like Cryptomator encrypt files before they reach the cloud.\n" +
                    "  • Review sharing links — don't share folders publicly if only specific people need access.\n" +
                    "  • Cloud providers operate on a shared responsibility model — security is a joint job."
            },

            // ── Children Online ───────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "children's online safety",
                Keywords  = new[] { "children", "kids", "child safety", "parental controls", "predator", "grooming" },
                Responses = new[]
                {
                    "Keeping children safe online requires active involvement, {name}!\n\n" +
                    "  • Use parental controls and content filters on all devices.\n" +
                    "  • Teach children never to share their name, school, or location online.\n" +
                    "  • Keep devices in common areas — not bedrooms.\n" +
                    "  • Maintain open, non-judgmental conversations about what they see and experience online."
                },
                FollowUp =
                    "More on children's online safety, {name}:\n\n" +
                    "  • Platforms like TikTok and Instagram have minimum age restrictions — enforce them.\n" +
                    "  • Online gaming can expose children to strangers — review friend/chat settings.\n" +
                    "  • Teach children that online friends are strangers until proven otherwise.\n" +
                    "  • Report suspected online grooming to the South African Police Service or CyberCrime unit."
            },

            // ── Software Updates ──────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "software updates",
                Keywords  = new[] { "update", "updates", "patch", "patching", "outdated", "security patch" },
                Responses = new[]
                {
                    "Keeping software updated is one of the easiest wins in security, {name}!\n\n" +
                    "  • Most attacks exploit known vulnerabilities with available patches.\n" +
                    "  • Enable automatic updates for your OS, browser, and apps.\n" +
                    "  • Don't ignore update notifications — they're often security critical.\n" +
                    "  • Remove software you no longer use — unused apps still pose vulnerabilities."
                },
                FollowUp =
                    "More on patching, {name}:\n\n" +
                    "  • Zero-day vulnerabilities are flaws with no patch yet — updating quickly after patches drop is crucial.\n" +
                    "  • Patch management tools help businesses keep hundreds of systems updated centrally.\n" +
                    "  • End-of-life (EOL) software no longer receives patches — time to upgrade.\n" +
                    "  • The Equifax breach (2017) exposed 147 million people due to an unpatched vulnerability."
            },

            // ── General Greeting ──────────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "greeting",
                Keywords  = new[] { "hello", "hi", "hey", "good morning", "good afternoon", "howzit", "sup" },
                Responses = new[]
                {
                    "Hello, {name}! 😊 Great to see you. What cybersecurity topic can I help you with today?",
                    "Hey {name}! Ready to level up your online safety? What would you like to know?",
                    "Hi {name}! Cybersecurity is my favourite topic. What would you like to explore today?"
                },
                FollowUp = "Just let me know what you'd like to learn about, {name}!"
            },

            // ── Thank You / Goodbye ───────────────────────────────────────────────
            new TopicEntry
            {
                TopicName = "goodbye",
                Keywords  = new[] { "thank", "thanks", "bye", "goodbye", "see you", "cheers", "appreciate", "exit" },
                Responses = new[]
                {
                    "You're very welcome, {name}! Stay safe out there. 🔒 Come back anytime!",
                    "Happy to help, {name}! Remember — good habits are the best security tool. Take care! 🛡️",
                    "Goodbye, {name}! Stay vigilant online. You're now better equipped than most people! 💪"
                },
                FollowUp = "Feel free to come back anytime, {name}!"
            }
        };
    }
}