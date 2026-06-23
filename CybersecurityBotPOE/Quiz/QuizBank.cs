namespace CybersecurityBotPOE.Quiz
{
    /// <summary>
    /// Contains all cybersecurity quiz questions.
    /// Mix of multiple-choice and true/false formats covering all major topics.
    /// </summary>
    public static class QuizBank
    {
        public static List<QuizQuestion> GetAllQuestions() => new()
        {
            // ── Multiple Choice ───────────────────────────────────────────────────

            new QuizQuestion
            {
                QuestionText = "What should you do if you receive an email asking for your password?",
                Options      = new[] { "A) Reply with your password", "B) Delete the email",
                                       "C) Report the email as phishing", "D) Ignore it" },
                CorrectIndex = 2,
                Explanation  = "You should always report phishing emails. Legitimate organisations never ask for your password via email."
            },
            new QuizQuestion
            {
                QuestionText = "What is the minimum recommended length for a strong password?",
                Options      = new[] { "A) 6 characters", "B) 8 characters",
                                       "C) 12 characters", "D) 4 characters" },
                CorrectIndex = 2,
                Explanation  = "Security experts recommend at least 12 characters. Longer passwords are exponentially harder to crack."
            },
            new QuizQuestion
            {
                QuestionText = "Which of the following is the SAFEST way to store your passwords?",
                Options      = new[] { "A) Write them on a sticky note", "B) Use the same password everywhere",
                                       "C) Use a reputable password manager", "D) Save them in a text file" },
                CorrectIndex = 2,
                Explanation  = "A password manager securely encrypts all your passwords. You only need to remember one strong master password."
            },
            new QuizQuestion
            {
                QuestionText = "What does '2FA' stand for?",
                Options      = new[] { "A) Two-Factor Authentication", "B) Two-File Access",
                                       "C) Two-Firewall Application", "D) Total Firewall Activation" },
                CorrectIndex = 0,
                Explanation  = "Two-Factor Authentication adds a second verification step beyond your password, greatly improving account security."
            },
            new QuizQuestion
            {
                QuestionText = "What is 'phishing'?",
                Options      = new[] { "A) A type of computer virus",
                                       "B) A trick to steal your info by pretending to be a trusted source",
                                       "C) A safe browsing technique",
                                       "D) A type of firewall" },
                CorrectIndex = 1,
                Explanation  = "Phishing tricks you into revealing passwords or personal info by impersonating banks, companies, or other trusted parties."
            },
            new QuizQuestion
            {
                QuestionText = "Which of these is the SAFEST network to use for online banking?",
                Options      = new[] { "A) Free airport Wi-Fi", "B) A coffee shop hotspot",
                                       "C) Your home network", "D) Any public Wi-Fi" },
                CorrectIndex = 2,
                Explanation  = "Your home network is password-protected and private. Public Wi-Fi can be monitored by anyone on the same network."
            },
            new QuizQuestion
            {
                QuestionText = "What does HTTPS indicate about a website?",
                Options      = new[] { "A) It is fast", "B) It uses encryption to protect your data",
                                       "C) It is government-owned", "D) It has no ads" },
                CorrectIndex = 1,
                Explanation  = "HTTPS uses TLS encryption to protect data between your browser and the website, preventing eavesdropping."
            },
            new QuizQuestion
            {
                QuestionText = "What is ransomware?",
                Options      = new[] { "A) Software that speeds up your PC",
                                       "B) Malware that encrypts your files and demands payment",
                                       "C) A type of antivirus",
                                       "D) A browser extension" },
                CorrectIndex = 1,
                Explanation  = "Ransomware locks your files and demands payment. Prevention (backups + updates) is the best defence. Never pay the ransom."
            },
            new QuizQuestion
            {
                QuestionText = "Which authentication method is MOST secure?",
                Options      = new[] { "A) SMS text code", "B) Email one-time password",
                                       "C) Authenticator app (TOTP)", "D) Security question" },
                CorrectIndex = 2,
                Explanation  = "Authenticator apps generate time-based codes that cannot be intercepted like SMS. Hardware keys are even stronger."
            },
            new QuizQuestion
            {
                QuestionText = "What is social engineering in cybersecurity?",
                Options      = new[] { "A) Building secure social media profiles",
                                       "B) Manipulating people psychologically to reveal sensitive information",
                                       "C) Engineering better social networks",
                                       "D) A programming technique" },
                CorrectIndex = 1,
                Explanation  = "Social engineering exploits human psychology rather than technical flaws. Always verify identities before sharing sensitive info."
            },

            // ── True / False ──────────────────────────────────────────────────────

            new QuizQuestion
            {
                QuestionText = "TRUE or FALSE: It is safe to reuse the same password across multiple websites.",
                Options      = new[] { "A) True", "B) False" },
                CorrectIndex = 1,
                IsTrueFalse  = true,
                Explanation  = "FALSE. If one site is breached, attackers will try your password on other sites (credential stuffing). Use unique passwords everywhere."
            },
            new QuizQuestion
            {
                QuestionText = "TRUE or FALSE: A VPN (Virtual Private Network) encrypts your internet traffic.",
                Options      = new[] { "A) True", "B) False" },
                CorrectIndex = 0,
                IsTrueFalse  = true,
                Explanation  = "TRUE. A VPN creates an encrypted tunnel for your traffic, hiding it from your ISP and others on the same network."
            },
            new QuizQuestion
            {
                QuestionText = "TRUE or FALSE: Antivirus software alone is enough to keep your computer safe.",
                Options      = new[] { "A) True", "B) False" },
                CorrectIndex = 1,
                IsTrueFalse  = true,
                Explanation  = "FALSE. Antivirus is just one layer. You also need updates, strong passwords, 2FA, safe browsing habits, and regular backups."
            },
            new QuizQuestion
            {
                QuestionText = "TRUE or FALSE: Private/Incognito mode hides your activity from your Internet Service Provider.",
                Options      = new[] { "A) True", "B) False" },
                CorrectIndex = 1,
                IsTrueFalse  = true,
                Explanation  = "FALSE. Incognito only prevents local browser history from being saved. Your ISP, employer, and websites can still see your activity."
            },
            new QuizQuestion
            {
                QuestionText = "TRUE or FALSE: You should always verify the sender's email address before clicking links in emails.",
                Options      = new[] { "A) True", "B) False" },
                CorrectIndex = 0,
                IsTrueFalse  = true,
                Explanation  = "TRUE. Phishing emails often use addresses that look similar to real ones. Always hover over links before clicking."
            },
            new QuizQuestion
            {
                QuestionText = "TRUE or FALSE: Software updates should be delayed as long as possible to avoid bugs.",
                Options      = new[] { "A) True", "B) False" },
                CorrectIndex = 1,
                IsTrueFalse  = true,
                Explanation  = "FALSE. Updates patch security vulnerabilities. Delaying them gives attackers time to exploit known weaknesses in unpatched software."
            },
        };

        /// <summary>Returns a shuffled subset of questions for a quiz session.</summary>
        public static List<QuizQuestion> GetShuffled(int count = 10)
        {
            var all = GetAllQuestions();
            var rng = new Random();
            return all.OrderBy(_ => rng.Next()).Take(Math.Min(count, all.Count)).ToList();
        }
    }
}