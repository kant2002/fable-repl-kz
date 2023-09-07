import { Some, value } from "./Util.js";
export { Some, value };
// Options are erased in runtime by Fable, but we have
// the `Some` type below to wrap values that would evaluate
// to `undefined` in runtime. These two rules must be followed:
// 1- `None` is always `undefined` in runtime, a non-strict null check
//    (`x == null`) is enough to check the case of an option.
// 2- To get the value of an option the `value` helper
//    below must **always** be used.
// Note: We use non-strict null check for backwards compatibility with
// code that use F# options to represent values that could be null in JS
export function some(x) {
    return x == null || x instanceof Some ? new Some(x) : x;
}
export function ofNullable(x) {
    // This will fail with unit probably, an alternative would be:
    // return x === null ? undefined : (x === undefined ? new Some(x) : x);
    return x == null ? undefined : x;
}
export function toNullable(x) {
    return x == null ? null : value(x);
}
export function flatten(x) {
    return x == null ? undefined : value(x);
}
export function toArray(opt) {
    return (opt == null) ? [] : [value(opt)];
}
export function defaultArg(opt, defaultValue) {
    return (opt != null) ? value(opt) : defaultValue;
}
export function defaultArgWith(opt, defThunk) {
    return (opt != null) ? value(opt) : defThunk();
}
export function orElse(opt, ifNone) {
    return opt == null ? ifNone : opt;
}
export function orElseWith(opt, ifNoneThunk) {
    return opt == null ? ifNoneThunk() : opt;
}
export function filter(predicate, opt) {
    return (opt != null) ? (predicate(value(opt)) ? opt : undefined) : opt;
}
export function map(mapping, opt) {
    return (opt != null) ? some(mapping(value(opt))) : undefined;
}
export function map2(mapping, opt1, opt2) {
    return (opt1 != null && opt2 != null) ? mapping(value(opt1), value(opt2)) : undefined;
}
export function map3(mapping, opt1, opt2, opt3) {
    return (opt1 != null && opt2 != null && opt3 != null) ? mapping(value(opt1), value(opt2), value(opt3)) : undefined;
}
export function bind(binder, opt) {
    return opt != null ? binder(value(opt)) : undefined;
}
export function tryOp(op, arg) {
    try {
        return some(op(arg));
    }
    catch (_a) {
        return undefined;
    }
}
