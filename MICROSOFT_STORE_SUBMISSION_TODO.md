# Microsoft Store Submission To-Do List

## 📝 Pre-Submission Setup

### 1. Partner Center Account
- [ ] Create Microsoft Partner Center account (one-time $19 registration fee)
  - URL: https://partner.microsoft.com/dashboard/registration
- [ ] Complete developer account verification (may take 24-48 hours)

### 2. Reserve App Name
- [ ] Log into Partner Center
- [ ] Navigate to: Apps and Games → New Product → MSIX or PWA app
  - URL: https://partner.microsoft.com/dashboard/apps-and-games/overview
- [ ] Reserve "ManpLab" or "ManpLab - Fractal Explorer" (name must be unique)
- [ ] Download the Store identity values (Publisher, Package Name, Publisher ID)

### 3. Update Package.appxmanifest
- [ ] Open `ManpWinUI\Package.appxmanifest`
- [ ] Update `<Identity>` section with Store values from Partner Center:
  - `Name` (from Store)
  - `Publisher` (from Store, format: `CN=...`)
  - Keep current `Version="1.4.0.0"`
- [ ] Update `<PublisherDisplayName>` with your real name/company
- [ ] Verify all icon assets are referenced correctly
- [ ] Rebuild MSIX package after changes

### 4. Privacy Policy (REQUIRED)
- [ ] Create privacy policy page (even if you collect no data)
  - Example: "ManpLab does not collect, store, or transmit any personal data"
- [ ] Host on GitHub Pages, personal website, or use Microsoft's simple template
- [ ] URL must be publicly accessible via HTTPS
- [ ] Template: https://docs.microsoft.com/windows/uwp/publish/app-privacy-policy

### 5. Store Listing Content
- [ ] Write detailed app description (500+ words recommended)
- [ ] Create feature list highlighting:
  - 300+ fractal types
  - Deep zoom capabilities
  - Real-time rendering
  - Animation support
  - No ads, no tracking
- [ ] Prepare short description (200 characters max)
- [ ] Write release notes for v1.4.0

### 6. Visual Assets (Screenshots & Images)
- [ ] Take 4-10 high-quality app screenshots (1366x768 or 1920x1080)
  - Show different fractals
  - Show UI features (zoom, color palettes, parameters)
  - Annotate key features if helpful
- [ ] Create Store logo (1240x600 PNG, for Store homepage)
- [ ] Optional: Create promotional images for marketing
- [ ] Optional: Create 30-second trailer video

### 7. Age Rating & Content
- [ ] Complete age rating questionnaire in Partner Center
  - ManpLab likely qualifies as "Everyone" (no violence, language, etc.)
- [ ] Declare app doesn't contain ads (if applicable)
- [ ] Declare in-app purchases: None

### 8. Testing
- [ ] Test MSIX installation on clean Windows 10 machine
- [ ] Test MSIX installation on clean Windows 11 machine
- [ ] Verify all fractals render correctly
- [ ] Test core features: zoom, colors, save, export
- [ ] Verify app uninstalls cleanly
- [ ] Check that icon displays in Start Menu

## 🚀 Submission Process

### 9. Upload Package
- [ ] In Partner Center, create new submission
- [ ] Upload rebuilt `ManpLab-v1.4.0-x64.msix` (with Store identity)
- [ ] System will validate package automatically
- [ ] Fix any validation errors

### 10. Complete Store Listing
- [ ] Add app description (from step 5)
- [ ] Add screenshots (from step 6)
- [ ] Add privacy policy URL (from step 4)
- [ ] Add support contact (email or website)
- [ ] Select app category: "Entertainment" or "Photo & Video"
- [ ] Add search keywords (e.g., "fractal", "mandelbrot", "visualization", "math")

### 11. Pricing & Distribution
- [ ] Set pricing: Free (recommended) or Paid
- [ ] Select markets: Worldwide or specific countries
- [ ] Set availability date: As soon as it passes certification

### 12. Submit for Certification
- [ ] Review all information
- [ ] Click "Submit to the Store"
- [ ] Wait for certification (typically 24-48 hours)
- [ ] Monitor certification status in Partner Center

### 13. Post-Certification
- [ ] Receive approval notification via email
- [ ] App goes live on Microsoft Store
- [ ] Share Store link: `https://www.microsoft.com/store/apps/[your-app-id]`
- [ ] Update GitHub README with Store badge
- [ ] Celebrate! 🎉

## 📚 Helpful Resources

### Official Documentation
- **Store Policies:** https://docs.microsoft.com/windows/apps/publish/store-policies
- **Submission Checklist:** https://docs.microsoft.com/windows/apps/publish/app-submissions
- **MSIX Packaging:** https://docs.microsoft.com/windows/msix/package/packaging-uwp-apps
- **Store Listing Best Practices:** https://docs.microsoft.com/windows/apps/publish/write-a-great-app-description

### Partner Center
- **Dashboard:** https://partner.microsoft.com/dashboard
- **Support:** https://developer.microsoft.com/windows/support

### Design Guidelines
- **Store Listing Assets:** https://docs.microsoft.com/windows/apps/publish/screenshots-and-images
- **App Icons:** https://docs.microsoft.com/windows/apps/design/style/app-icons-and-logos

## ⏱️ Timeline Estimate

- Partner Center account setup: **1-3 days** (verification time)
- Preparing assets and content: **2-4 hours**
- Package updates and testing: **2-3 hours**
- Store submission: **15 minutes**
- Certification review: **24-48 hours** (typically)
- **Total:** ~3-5 days from start to live

## 💡 Tips

- Name reservation is free but name must be unique globally
- Microsoft signs your package during certification (no need for your own certificate)
- You can update listings (description, screenshots) anytime without recertification
- Code updates require new certification review
- First submission typically takes longer than updates
- Store provides analytics after app is live

## ⚠️ Common Issues

- **Certification failure:** Usually fixable within hours (they tell you exactly what's wrong)
- **Name already taken:** Have backup names ready
- **Privacy policy missing:** Most common rejection reason
- **Screenshots don't match app:** Make sure images are from current version

---

**Current Status:** Pre-submission  
**Next Action:** Create Partner Center account and reserve app name  
**Questions?** Check Microsoft docs or contact Partner Center support
