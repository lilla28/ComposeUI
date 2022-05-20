"use strict";
/**
 * The descriptors are used to describe what properties/functions to expose on an object
 * and what are the function return behaviors.
 * @module
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.isFunctionDescriptor = exports.getPropertyDescriptor = exports.getFunctionDescriptor = exports.getArgumentDescriptor = exports.getPropName = void 0;
// util functions
function getPropName(descriptor) {
    return typeof descriptor === 'string' ? descriptor : descriptor.name || '';
}
exports.getPropName = getPropName;
function getArgumentDescriptor(descriptor, idx) {
    return typeof descriptor === 'object' ? descriptor.arguments?.find(arg => arg.idx == null || arg.idx === idx) : undefined;
}
exports.getArgumentDescriptor = getArgumentDescriptor;
function getFunctionDescriptor(descriptor, funcName) {
    return descriptor?.functions?.find(func => typeof func === 'object' && func.name === funcName);
}
exports.getFunctionDescriptor = getFunctionDescriptor;
function getPropertyDescriptor(descriptor, propName) {
    return descriptor?.proxiedProperties?.find(prop => typeof prop === 'object' && prop.name === propName);
}
exports.getPropertyDescriptor = getPropertyDescriptor;
function isFunctionDescriptor(descriptor) {
    return descriptor?.type === 'function';
}
exports.isFunctionDescriptor = isFunctionDescriptor;
//# sourceMappingURL=rpc-descriptor-types.js.map