import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function handleDownload(data: Blob, fileName: string | null | undefined = 'file') {
  // Ensure 'data' is a Blob
  if (!(data instanceof Blob)) {
    console.error('Expected a Blob object');
    return;
  }

  const url = window.URL.createObjectURL(data);
  const contentType = data.type; // Get the content type (MIME type)

  // Determine the appropriate file name based on the content type
  fileName = fileName && fileName.trim().length > 0 ? fileName : 'file';

  switch (contentType) {
    case 'application/pdf':
      fileName += '.pdf';
      break;
    case 'image/png':
      fileName += '.png';
      break;
    case 'image/jpeg':
      fileName += '.jpg';
      break;
    case 'application/json':
      fileName += '.json';
      break;
    case 'text/plain':
      fileName += '.txt';
      break;
    case 'text/html':
      fileName += '.html';
      break;
    default:
      fileName += '.bin'; // Fallback for unknown types
      console.warn(`Unsupported file type: ${contentType}. Using fallback: ${fileName}.bin`);
      break;
  }

  const a = document.createElement('a');
  a.style.display = 'none';
  a.href = url;
  a.download = fileName; // Set the determined file name
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a); // Clean up the DOM
  window.URL.revokeObjectURL(url); // Clean up the object URL
}
