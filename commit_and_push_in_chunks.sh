#!/bin/bash

# Directory to process
DIR="./MI498-SP23-Bungie/"

# Temporary file to hold list of files for each chunk
TEMP_FILE="temp_chunk_list.txt"

# Counter for total size and number of commits
TOTAL_SIZE=0
COMMIT_NUM=1

# Iterate over each file in the directory (and its subdirectories)
find "$DIR" -type f | while read file; do
  
  # Only process files not tracked by git (or modified ones)
  if ! git ls-files --error-unmatch "$file" &> /dev/null; then

    # Get the size of the file in kilobytes
    FILE_SIZE=$(du -k "$file" | cut -f1)

    # Add the file size to the total size
    TOTAL_SIZE=$((TOTAL_SIZE + FILE_SIZE))

    # If the total size is greater than 10MB (i.e., 10240KB), commit and push the files
    if [ "$TOTAL_SIZE" -gt 10240 ]; then
      echo "Committing and pushing chunk $COMMIT_NUM..."
      git add $(<"$TEMP_FILE")
      git commit -m "Adding chunk $COMMIT_NUM of files"
      git push origin main
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
  git add $(<"$TEMP_FILE")
  git commit -m "Adding final chunk $COMMIT_NUM of files"
  git push origin main
fi

# Cleanup
rm -f "$TEMP_FILE"

