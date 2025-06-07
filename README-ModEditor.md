# Memoria Mod Catalog Editor

A web-based editor for the Memoria Mod Catalog that allows users to edit, create, and manage mods directly from a GitHub Pages interface.

## Features

- 🔍 **Browse and Search**: View all mods with searchable dropdowns
- ✏️ **Edit Existing Mods**: Modify any field of existing mods and sub-mods
- ➕ **Create New Content**: Add new mods or sub-mods to the catalog
- 🔒 **GitHub Integration**: Secure authentication and automatic Pull Request creation
- 🌙 **Dark Theme**: Beautiful dark theme optimized for developers
- ✅ **Validation**: Comprehensive validation for required fields
- 📱 **Responsive**: Works on desktop and mobile devices

## How to Use

### 1. Authentication

1. Click "Login with GitHub"
2. Create a Personal Access Token:
   - Go to GitHub Settings > Developer settings > Personal access tokens > Tokens (classic)
   - Click "Generate new token"
   - Required scopes: `repo`, `workflow`
   - Copy the generated token
3. Paste the token in the prompt

### 2. Editing Existing Mods

1. Select a mod from the "Select Mod" dropdown
2. Choose "Main" to edit the main mod, or select a specific sub-mod
3. Edit the fields as needed
4. Click "Save Changes" to create a Pull Request

### 3. Creating New Mods

1. Click "Create New Mod"
2. Fill in all required fields (marked with *)
3. Add optional fields as needed
4. Click "Save Changes" to create a Pull Request

### 4. Creating New Sub-Mods

1. Select the parent mod from "Select Mod"
2. Click "Create New SubMod"
3. Fill in the sub-mod details
4. Click "Save Changes" to create a Pull Request

## Deployment

This editor is designed to be deployed on GitHub Pages:

1. Place `index.html` in the root of your `gh-pages` branch
2. Enable GitHub Pages in repository settings
3. The editor will be available at `https://[username].github.io/[repository]/`

## Security

- Uses Personal Access Tokens for authentication (no OAuth secrets needed)
- All changes create Pull Requests that require review
- No direct write access to the main branch
- Tokens are stored locally and never transmitted to third parties

## Supported Fields

### Main Mods
- **Required**: Name, Version, Installation Path, Author, Description
- **Optional**: Priority, Category, Release Date, Website, Download URL, Preview File URL, Patch Notes, Incompatible With, Minimum Memoria Version

### Sub-Mods
- **Required**: Name, Description
- **Optional**: Version, Installation Path, all other fields from main mods

## Technical Details

- Pure client-side application (HTML, CSS, JavaScript)
- Uses GitHub API for all operations
- Bootstrap 5 for responsive UI
- Select2 for enhanced dropdowns with search
- XML parsing and manipulation with native DOM APIs

## Browser Compatibility

- Chrome/Edge 88+
- Firefox 78+
- Safari 14+

## Contributing

This editor automatically creates Pull Requests for all changes. Repository maintainers will review and merge approved modifications.

## Troubleshooting

### "Error loading mod catalog"
- Check your internet connection
- Verify the repository and file path are correct

### "Failed to create pull request"
- Ensure your Personal Access Token has `repo` scope
- Check that you haven't exceeded GitHub API rate limits
- Verify the token hasn't expired

### "Authentication failed"
- Generate a new Personal Access Token
- Ensure the token has the required scopes
- Check for typos when entering the token

## License

This editor follows the same license as the Memoria project.