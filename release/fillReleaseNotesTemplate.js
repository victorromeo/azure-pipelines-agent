const fs = require('fs');
const path = require('path');
const util = require('./util');

function addHashesToReleaseNotes(releaseNotes) {
    const hashes = util.getHashes();

    const lines = releaseNotes.split('\n');
    const modifiedLines = lines.map((line) => {
        if (!line.includes('<HASH>')) {
            return line;
        }

        // Package is the second column in the releaseNote.md file, get it's value
        const columns = line.split('|').filter((column) => column.length !== 0);
        const packageColumn = columns[1];
        // Inside package column, we have the package name inside the square brackets
        const packageName = packageColumn.substring(packageColumn.indexOf('[') + 1, packageColumn.indexOf(']'));

        return line.replace('<HASH>', hashes[packageName]);
    });

    return modifiedLines.join('\n');
}

function addAgentVersionToReleaseNotes(releaseNotes, agentVersion) {
    return releaseNotes.replace(/<AGENT_VERSION>/g, agentVersion);
}

function main() {
    const agentVersion = process.argv[2];
    if (agentVersion === undefined) {
        throw new Error('Agent version argument must be supplied');
    }

    const releaseNotesPath = path.join(__dirname, '..', 'releaseNote.md');
    const releaseNotes = fs.readFileSync(releaseNotesPath, 'utf-8');

    const releaseNotesWithAgentVersion = addAgentVersionToReleaseNotes(releaseNotes, agentVersion);
    const filledReleaseNotes = addHashesToReleaseNotes(releaseNotesWithAgentVersion);
    console.log(filledReleaseNotes);
    fs.writeFileSync(releaseNotesPath, filledReleaseNotes);
}

main();
