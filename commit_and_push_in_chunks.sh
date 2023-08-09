#!/bin/bash

# Directory to process
DIR="./MI498-SP23-Bungie/"

# Temporary file to hold list of files for each chunk
TEMP_FILE="temp_chunk_list.txt"

# Counter for total size and number of commits
TOTAL_SIZE=0
COMMIT_NUM=1

# Function to add file extension to .gitattributes for LFS tracking
add_to_gitattributes() {
    FILE_EXTENSION=".$1"
    if ! grep -q "\*${FILE_EXTENSION} filter=lfs diff=lfs merge=lfs -text" .gitattributes; then
        echo "*${FILE_EXTENSION} filter=lfs diff=lfs merge=lfs -text" >> .gitattributes
    fi
}

add_files_and_commit() {
    while IFS= read -r file; do
        git add "$file"
    done < "$TEMP_FILE"
    git commit -m "Adding chunk $COMMIT_NUM of files"
    git push origin main
}

# Iterate over each file in the directory (and its subdirectories)
find "$DIR" -type f | while read file; do
  
  # Only process files not tracked by git (or modified ones)
  if ! git ls-files --error-unmatch "$file" &> /dev/null; then

    # Get the size of the file in kilobytes
    FILE_SIZE=$(du -k "$file" | cut -f1)

    # Check if the file size exceeds 50MB (i.e., 51200KB) and add its extension to .gitattributes
    if [ "$FILE_SIZE" -gt 51200 ]; then
        EXTENSION="${file##*.}"
        add_to_gitattributes "$EXTENSION"
    fi

    # Add the file size to the total size
    TOTAL_SIZE=$((TOTAL_SIZE + FILE_SIZE))

    # If the total size is greater than 50MB (i.e., 51200KB), commit and push the files
    if [ "$TOTAL_SIZE" -gt 51200 ]; then
      echo "Committing and pushing chunk $COMMIT_NUM..."
      add_files_and_commit
      COMMIT_NUM=$((COMMIT_NUM+1))

      # Reset the total size and the temp file
      TOTAL_SIZE=0
      > "$TEMP_FILE"
    fi
    
    # Append the current file to the temp file
    echo "$file" >> "$TEMP_FILE"
  fi
done

# Handle any remaining files
if [ "$TOTAL_SIZE" -gt 0 ]; then
  echo "Committing and pushing final chunk $COMMIT_NUM..."
  add_files_and_commit
fi

# Cleanup
rm -f "$TEMP_FILE"

