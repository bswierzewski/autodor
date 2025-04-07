import { Button } from './ui/button';
import { useConfirmationStore } from '@/stores/confirmation';

import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/dialog';

const ConfirmationDialog = () => {
  const { open, title, description, cancelLabel, actionLabel, onAction, onCancel, closeConfirmation } =
    useConfirmationStore();

  return (
    <Dialog open={open} onOpenChange={closeConfirmation}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <DialogClose asChild onClick={onCancel}>
            <Button variant="outline">{cancelLabel}</Button>
          </DialogClose>
          <DialogClose asChild onClick={onAction}>
            <Button variant="destructive">{actionLabel}</Button>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default ConfirmationDialog;
