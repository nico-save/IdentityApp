export interface MemberView {
  id: string;
  username: string;
  lastname: string;
  firstname: string;
  dateCreated: Date;
  isLocked: boolean;
  roles: string[];
}
